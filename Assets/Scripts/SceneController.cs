using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneController : MonoBehaviour
{
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();
    private static readonly List<ARAnchor> Anchors = new List<ARAnchor>();
    [SerializeField] private ARRaycastManager m_RaycastManager;
    [SerializeField] private ARPlaneManager m_PlaneManager;
    [SerializeField] private ARAnchorManager m_AnchorManager;

    [SerializeField] private SettingsController _settings;
    [SerializeField] private ScoreboardController _scoreboardController;
    [SerializeField] private SnakeController _snakeController;
    [SerializeField] private FoodController _foodController;

    [SerializeField] private GameObject m_ObjectToPlace;

    [SerializeField] private Material m_Faded;

    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private ARSession _arSession;

    [SerializeField] private Text _debugText;

    // --------------------------------
    // --------------------------------
    // BUTTON CONTROLS
    // --------------------------------
    // --------------------------------

    public void StartGameButton()
    {
        _mainCanvas.SetActive(false);
        _gameCanvas.SetActive(true);
        _gameState = GameState.GameIntro;
    }

    public void ResumeGame()
    {
        _pauseCanvas.SetActive(false);
        _gameCanvas.SetActive(true);
    }

    public void Settings()
    {
        _mainCanvas.SetActive(false);
        _settingsCanvas.SetActive(true);
    }

    public void SettingsReset()
    {
        _mainCanvas.SetActive(false);
        _settingsCanvas.SetActive(true);
        _settings.Reset();
    }

    public void Pause()
    {
        _gameCanvas.SetActive(false);
        _pauseCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        _arSession.enabled = false;
        _pauseCanvas.SetActive(false);
        _mainCanvas.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Save()
    {
        _settings.UpdateSettings();
        _settingsCanvas.SetActive(false);
        _mainCanvas.SetActive(true);
    }

    // --------------------------------
    // --------------------------------
    // --------------------------------
    // --------------------------------

    private enum GameState
    {
        Menu,
        GameIntro,
        GameSelectedPlane,
        GamePlaying
    }

    private GameState _gameState = GameState.Menu;
    private bool _gameStarted;

    // Update is called once per frame
    private void Update()
    {
        switch (_gameState)
        {
            case GameState.Menu:
                break;
            case GameState.GameIntro:
                _scoreboardController.Intro();
                if (_scoreboardController.finishedAnimation) _arSession.enabled = true;
                if (_arSession.enabled) ProcessInput();
                break;
            case GameState.GameSelectedPlane:
                if (_gameStarted) return;
                _gameStarted = true;
                StartCoroutine(StartGame());
                break;
            case GameState.GamePlaying:
                _scoreboardController.SetScore(_snakeController.GetLength());
                SetGazePointer();
                UpdateAISnakes();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator StartGame()
    {
        _scoreboardController.SetText("Get Ready!");
        yield return new WaitForSeconds(1);
        _scoreboardController.SetText("3");
        yield return new WaitForSeconds(1);
        _scoreboardController.SetText("2");
        yield return new WaitForSeconds(1);
        _scoreboardController.SetText("1");
        yield return new WaitForSeconds(1);
        _scoreboardController.SetText("Go!");
        yield return new WaitForSeconds(1);
        _snakeController.StartGame();
        _foodController.StartGame();
        _scoreboardController.StartGame();
        _gameState = GameState.GamePlaying;
    }

    private void SetSelectedPlane(ARPlane arPlane)
    {
        _snakeController.SetSelectedPlane(arPlane);
        _foodController.SetSelectedPlane(arPlane);
        _gameState = GameState.GameSelectedPlane;
    }

    private void UpdateAISnakes()
    {
        _snakeController.UpdateFoodKnowledge(_foodController.GetFoodPositions());
        _snakeController.UpdateAIs();
    }

    private void SetGazePointer()
    {
        if (!m_RaycastManager.Raycast(new Vector2(Screen.width / 2f, Screen.height / 2f), Hits, TrackableType.PlaneWithinPolygon)) return;
        var pointer = Hits[0].pose.position;
        _snakeController.SetPointer(pointer);
    }

    private void ProcessInput()
    {
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;
        if (m_RaycastManager.Raycast(touch.position, Hits, TrackableType.PlaneWithinPolygon))
        {
            var raycastHit = Hits[0];
            var anchor = m_AnchorManager.AttachAnchor(raycastHit.trackable as ARPlane, raycastHit.pose);
            if (anchor != null)
            {
                RemoveAllAnchors();
                Anchors.Add(anchor);
            }

            SetSelectedPlane(raycastHit.trackable as ARPlane);
            
            foreach (var trackable in m_PlaneManager.trackables)
                trackable.gameObject.GetComponent<MeshRenderer>().material = m_Faded;
        }
    }

    private void RemoveAllAnchors()
    {
        foreach (var anchor in Anchors)
            Destroy(anchor);
        Anchors.Clear();
    }
}