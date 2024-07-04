using System.Collections.Generic;
using Battle;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    // Wait,
    // Move,
    Playing,
    Win,
    Lose,
    Pause,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Ins;
    
    [Header("Game state")] private GameState _currentState = GameState.Playing;
    public GameState CurrentState => _currentState;

    public BoardManager boardManager;
    public BattleSystem battleSystem;

    private void Awake()
    {
        if (Ins == null)
            Ins = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        Reset();
        boardManager.Initialize();
    }

    private void Reset()
    {
        _currentState = GameState.Playing;
    }
    
    
}