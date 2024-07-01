using UnityEngine;

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
    [Header("Game state")] public GameState CurrentState = GameState.Move;

    public BoardManager boardManager;

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
        CurrentState = GameState.Move;
    }
}