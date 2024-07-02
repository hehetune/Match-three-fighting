using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    Wait,
    Move,
    Win,
    Lose,
    Pause,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Ins;
    public UIManager uiManager;
    [Header("Game state")] private GameState _currentState = GameState.Move;
    public GameState CurrentState => _currentState;

    public BoardManager boardManager;

    private bool _isPlayerTurn = true;

    public bool IsPlayerTurn
    {
        get => _isPlayerTurn;
    }

    public void ReceiveMatchResult(List<MatchResult> results)
    {
        ToggleTurn();
    }

    public void ToggleTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        _currentState = GameState.Move;
        uiManager.UpdateUIByTurn(_isPlayerTurn);
    }

    public void StartTurn()
    {
        _currentState = GameState.Wait;
    }

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
        _currentState = GameState.Move;
    }
}