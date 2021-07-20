using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private TextMeshProUGUI target;
    private RectTransform _rectTransform;

    public bool finishedAnimation;
    private bool _gameOver;
    private float _originalHeight;
    private bool _called;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalHeight = _rectTransform.rect.height;
    }

    public void StartGame()
    {
        textMesh.alignment = TextAlignmentOptions.TopLeft;
        target.text = "Target: 20";
        target.enabled = true;
    }

    private static void SetRect(RectTransform trs, float left, float top, float right, float bottom)
    {
        trs.offsetMin = new Vector2(left, bottom);
        trs.offsetMax = new Vector2(-right, -top);
    }

    public void Intro()
    {
        if (_called) return;
        _called = true;
        textMesh.text = "Scan around and select an AR Plane!";
        StartCoroutine(ShrinkCoroutine());
    }

    private IEnumerator ShrinkCoroutine()
    {
        yield return new WaitForSeconds(1);
        const float shrinkTime = 1f;
        var elapsedTime = 0f;
        while (elapsedTime < shrinkTime)
        {
            SetRect(_rectTransform, 0, (elapsedTime / shrinkTime) * (_originalHeight - 512), 0, 0);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        finishedAnimation = true;
    }

    public void SetScore(int score)
    {
        if (_gameOver) return;
        textMesh.text = "Round: 1\nScore: " + score;
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void GameOver(int score)
    {
        textMesh.text = "Game Over!\nFinal Score: " + score;
        _gameOver = true;
    }
}