using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    //Default values
    [SerializeField] private int _totalMoves = 5;
    [SerializeField] private int _pointsPerTile = 1;
    
    //Prefabs
    [SerializeField] private Transform _gameSceneCanvas;
    [SerializeField] private HUDManager _hudManagerPrefab;
    
    public static GameManager Instance { get; private set; }
    
    private event Action<int> ScoreUpdatedEvent;
    private event Action<int> MovesUpdatedEvent;
    private event Action GameoverEvent;
    private const int DEFAULT_SCORE = 0;
    private int _currentMoves;
    private int _currentScore;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    private void Start()
    {
        _currentMoves = _totalMoves;
        _currentScore = DEFAULT_SCORE;
        var hudInstance = Instantiate(_hudManagerPrefab, _gameSceneCanvas);
        hudInstance.Init(ref ScoreUpdatedEvent, ref MovesUpdatedEvent, ref GameoverEvent);
        
        MovesUpdatedEvent?.Invoke(_currentMoves);
        ScoreUpdatedEvent?.Invoke(_currentScore);
    }

    private bool IsGameover()
    {
        return _currentMoves == 0;
    }

    public void Replay()
    {
        _currentMoves = _totalMoves;
        _currentScore = DEFAULT_SCORE;
        
        MovesUpdatedEvent?.Invoke(_currentMoves);
        ScoreUpdatedEvent?.Invoke(_currentScore);
    }
    
    #region MakeMoveRegion
    public void MakeMove()
    {
        _currentMoves--;
        _currentScore += 10;
        
        MovesUpdatedEvent?.Invoke(_currentMoves);
        ScoreUpdatedEvent?.Invoke(_currentScore);

        if (IsGameover())
        {
            GameoverEvent?.Invoke();
        }
        
        
    }
    #endregion
}
