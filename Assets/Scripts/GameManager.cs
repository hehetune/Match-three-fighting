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
    public static GameManager Instance;
    [Header("Game state")]
    public GameState CurrentState = GameState.Move;
    
    private void Awake()
    {
        GameManager.Instance = this;
        
    }
}