using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SnakeController : MonoBehaviour
{
    private const float cellSize = 0.1f;
    [SerializeField] private FoodController foodController;
    [SerializeField] private ScoreboardController scoreboardController;
    [SerializeField] private GameController gameController;
    [SerializeField] private Camera m_FirstPersonCamera;
    [SerializeField] private GameObject m_Pointer;
    [SerializeField] private GameObject m_SnakeHead;
    [SerializeField] private float m_Speed = 20;

    [SerializeField] private Text _debugText;

    private readonly int AiSnakeCount = 4;
    // private List<GameObject> _AIPointers;

    private GameObject _playerSnake;
    private List<GameObject> _AISnakes;

    private List<Vector3> _foodPositions;

    private ARPlane m_DetectedPlane;
    private List<GridPoint> orderedQueue;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // private void Update()
    // {
    //     // for (var i = 0; i < AiSnakeCount; i++) _AIPointers[i].SetActive(_AISnakes != null);
    // }

    public void SetSelectedPlane(ARPlane plane)
    {
        m_DetectedPlane = plane;
        SpawnPlayerSnake();
        SpawnAISnakes();
    }

    public void StartGame()
    {
        m_Pointer.SetActive(true);
    }

    private void SpawnPlayerSnake()
    {
        if (_playerSnake != null) Destroy(_playerSnake);

        var center = m_DetectedPlane.center;
        _playerSnake = Instantiate(m_SnakeHead, center, Quaternion.identity, transform);
        _playerSnake.tag = "Player";
        var foodConsumer = _playerSnake.GetComponent<FoodConsumer>();
        foodConsumer.foodController = foodController;
        foodConsumer.scoreboardController = scoreboardController;
        foodConsumer.gameController = gameController;
        var slithering = _playerSnake.GetComponent<Slithering>();
        slithering.fixedTransform = transform;
        slithering.SetHead(false, _playerSnake.transform);
    }

    private void SpawnAISnakes()
    {
        if (_AISnakes != null)
            foreach (var aiSnake in _AISnakes)
                Destroy(aiSnake);

        _AISnakes = new List<GameObject>();
        // _AIPointers = new List<GameObject>();

        var center = m_DetectedPlane.center;
        var snakePositions = new List<Vector3>
        {
            new Vector3(center.x + 0.4f, center.y, center.z + 0.4f),
            new Vector3(center.x + 0.4f, center.y, center.z - 0.4f),
            new Vector3(center.x - 0.4f, center.y, center.z + 0.4f),
            new Vector3(center.x - 0.4f, center.y, center.z - 0.4f)
        };

        for (var i = 0; i < AiSnakeCount; i++)
        {
            var snake = Instantiate(m_SnakeHead, snakePositions[i], Quaternion.identity, transform);
            snake.tag = "AI";

            var color = UnityEngine.Random.ColorHSV(0.0f, 1f, 0.8f, 1f, 0.8f, 1f, 1f, 1f) * 2.5f;
            var foodConsumer = snake.GetComponent<FoodConsumer>();
            foodConsumer.foodController = foodController;
            var slithering = snake.GetComponent<Slithering>();
            slithering.fixedTransform = transform;
            slithering.color = color;
            slithering.SetHead(true, snake.transform);
            _AISnakes.Add(snake);
            // _AIPointers.Add(Instantiate(m_Pointer));
        }
    }

    public void SetPointer(Vector3 ptr)
    {
        if (!m_Pointer.activeSelf) return;

        ptr.y = _playerSnake.transform.position.y;
        var pos = m_Pointer.transform.position;
        pos.y = ptr.y;

        m_Pointer.transform.position = Vector3.Lerp(pos, ptr, Time.smoothDeltaTime * m_Speed);

        var dist = Vector3.Distance(m_Pointer.transform.position, _playerSnake.transform.position) - .05f;
        if (dist < 0) dist = 0;

        var rb = _playerSnake.GetComponent<Rigidbody>();
        rb.transform.LookAt(m_Pointer.transform.position);
        rb.velocity = _playerSnake.transform.forward * (_playerSnake.transform.localScale.x * dist) / .01f;
    }

    public void UpdateAIs()
    {
        if (_foodPositions.Count == 0)
            for (var i = 0; i < AiSnakeCount; i++)
            {
                var rigidBody = _AISnakes[i].GetComponent<Rigidbody>();
                rigidBody.velocity = 0.9f * rigidBody.velocity.magnitude * _AISnakes[i].transform.forward;
            }
        else
            for (var i = 0; i < AiSnakeCount; i++)
            {
                var rigidBody = _AISnakes[i].GetComponent<Rigidbody>();

                var closestFoodDistance = float.MaxValue;
                var closestFood = _foodPositions[0];
                foreach (var foodPosition in _foodPositions)
                {
                    var distance = Vector3.Distance(rigidBody.position, foodPosition);
                    if (distance < closestFoodDistance)
                    {
                        closestFoodDistance = distance;
                        closestFood = foodPosition;
                    }
                }

                rigidBody.transform.LookAt(closestFood);
                rigidBody.velocity = _AISnakes[i].transform.forward * (_AISnakes[i].transform.localScale.x * .1f) / .01f;
            }


        // calculate pointers
        // for (var i = 0; i < AiSnakeCount; i++)
        //     if (_AIPointers[i].activeSelf)
        //         _AIPointers[i].transform.position = CalculateAiSnakeDestination(i);

        // move snakes
        // for (var i = 0; i < AiSnakeCount; i++)
        // {
        //     var dist = Vector3.Distance(_AIPointers[i].transform.position, _AISnakes[i].transform.position) - .05f;
        //     if (dist < 0) dist = 0;
        //
        //     var rb = _AISnakes[i].GetComponent<Rigidbody>();
        //     rb.transform.LookAt(_AIPointers[i].transform.position);
        //     rb.velocity = _AISnakes[i].transform.forward * (_AISnakes[i].transform.localScale.x * .15f) / .01f;
        // }
    }

    public void UpdateFoodKnowledge(List<Vector3> foodPositions)
    {
        _foodPositions = foodPositions;
    }


    public int GetLength()
    {
        return _playerSnake != null ? _playerSnake.GetComponent<Slithering>().GetLength() : 0;
    }

    // private void CalculateAStarPath(int snakeId)
    // {
    //     var path = new List<Vector3>();
    //
    //     var startPoint = ToGrid(_AISnakes[snakeId].transform.position);
    //     var endPoint = ToGrid(_AIPointers[snakeId].transform.position);
    //
    //     path.Add(startPoint);
    //
    //     if (startPoint == endPoint)
    //         return;
    //
    //     var gridWidth = Convert.ToInt32(Math.Floor(startPoint.x - endPoint.x) / cellSize);
    //     var gridHeight = Convert.ToInt32(Math.Floor(startPoint.x - endPoint.x) / cellSize);
    //     if (gridWidth < 0) gridWidth *= -1;
    //     if (gridHeight < 0) gridHeight *= -1;
    //     gridWidth += 3;
    //     gridHeight += 3;
    //
    //     var gridX = Math.Min(startPoint.x, endPoint.x) - cellSize;
    //     var gridZ = Math.Min(startPoint.z, endPoint.z) - cellSize;
    //
    //     var grid = new bool[gridWidth, gridHeight];
    //
    //     var startX = Convert.ToInt32(Math.Round((startPoint.x - gridX) / cellSize));
    //     var startZ = Convert.ToInt32(Math.Round((startPoint.z - gridZ) / cellSize));
    //
    //     var endX = Convert.ToInt32(Math.Round((startPoint.x - gridX) / cellSize));
    //     var endZ = Convert.ToInt32(Math.Round((startPoint.z - gridZ) / cellSize));
    //
    //     var obstacles = new List<Vector3>();
    //
    //     for (var i = 0; i < AiSnakeCount; i++)
    //         if (i != snakeId)
    //         {
    //             var addedCords = _AISnakes[i].GetComponent<Slithering>().GetWholeBodyCoords();
    //             for (var j = 0; j < addedCords.Count; j++) obstacles.Add(addedCords[j]);
    //         }
    //
    //     for (var i = 0; i < obstacles.Count; i++)
    //     {
    //         obstacles[i] = ToGrid(obstacles[i]);
    //         var obstX = Convert.ToInt32(Math.Round((obstacles[i].x - gridX) / cellSize));
    //         var obstZ = Convert.ToInt32(Math.Round((obstacles[i].z - gridZ) / cellSize));
    //
    //         grid[obstX, obstZ] = true;
    //     }
    //
    //     orderedQueue = new List<GridPoint>();
    // }

    private Vector3 ToGrid(Vector3 old)
    {
        var newV = new Vector3(old.x, old.y, old.z);
        newV.x += (float) -decimal.Remainder(Convert.ToDecimal(newV.x), Convert.ToDecimal(cellSize)) + cellSize / 2.0f;
        newV.z += (float) -decimal.Remainder(Convert.ToDecimal(newV.z), Convert.ToDecimal(cellSize)) + cellSize / 2.0f;
        return newV;
    }

    private struct GridPoint
    {
        public int x;
        public int z;

        public GridPoint(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }

    private enum Destination
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }
}