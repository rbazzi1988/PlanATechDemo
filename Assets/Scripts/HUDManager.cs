using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreAmount;
    [SerializeField] private TextMeshProUGUI _movesAmount;
    [SerializeField] private GameObject _gameoverScreen;
    [SerializeField] private Button _makeMoveBtn;
    [SerializeField] private Button _replayBtn;
    public void Init(ref Action<int> scoreUpdated, ref Action<int> movesUpdated, ref Action gameOver)
    {
        scoreUpdated += ScoreUpdated;
        movesUpdated += MovesUpdated;
        gameOver += GameOver;
        _makeMoveBtn.onClick.AddListener(OnMakeMoveClicked);
        _replayBtn.onClick.AddListener(OnReplayClicked);
    }

    private void MovesUpdated(int moves)
    {
        _movesAmount.text = moves.ToString();
    }

    private void ScoreUpdated(int score)
    {
        _scoreAmount.text = score.ToString();
    }
    private void GameOver()
    {
        if (_gameoverScreen.activeInHierarchy == false)
        {
            _gameoverScreen.SetActive(true);
        }
    }

    private void OnReplayClicked()
    {
        GameManager.Instance.Replay();
        _gameoverScreen.SetActive(false);
    }
    
    #region MakeMoveBtnCode

    private void OnMakeMoveClicked()
    {
        GameManager.Instance.MakeMove();
    }
    #endregion
    
}
