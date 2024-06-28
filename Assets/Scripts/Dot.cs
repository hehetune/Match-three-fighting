using System.Collections;
using Patterns.ObserverPattern;
using UnityEngine;
using UnityEngine.Serialization;

public enum DotType
{
    Red = 0,
    Blue = 1,
    Yellow = 2,
    Purple = 3,
}

public enum DotState
{
    Idle = 0,
    Moving = 1,
}

public class Dot : MonoBehaviour, IObserver
{
    [Header("Board")]
    public BoardManager boardManager;

    [Header("Interact")]
    private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;

    [Header("Swipe")]
    public float swipeAngle;
    public float swipeResist = .5f;

    [Header("Dots around")] public Dot upDot;
    public Dot downDot;
    public Dot leftDot;
    public Dot rightDot;
    
    [Header("Dot properties")]
    public int col;
    public int row;
    public DotType dotType;

    public Vector3 targetPosition;

    public bool isMatch = false;
    
    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        Subject.Register(this, EventKey.DotExplode);
    }
        
    private void UnregisterEvents()
    {
        Subject.Unregister(this, EventKey.DotExplode);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState == GameState.Move)
        {
            _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.CurrentState == GameState.Move)
        {
            _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > swipeResist ||
            Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > swipeResist)
        {
            GameManager.Instance.CurrentState = GameState.Wait;
            swipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y,
                                     _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;

            HandleSwipe();
        }
        else
        {
            GameManager.Instance.CurrentState = GameState.Move;
        }
    }

    public void MoveToNode(Vector3 targetPosition)
    {
        boardManager.OnDotStartMoving();
        this.targetPosition = targetPosition;
        StartCoroutine(MoveToPositionCoroutine());
    }

    private IEnumerator MoveToPositionCoroutine()
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, 0.2f);
            yield return null;
        }

        transform.position = targetPosition;
        boardManager.OnDotStopMoving();
    }

    private void HandleSwipe()
    {
        Vector2Int direction = Vector2Int.zero;

        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            direction = Vector2Int.right;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            direction = Vector2Int.up;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            direction = Vector2Int.left;
        }
        else if (swipeAngle > -135 && swipeAngle <= -45)
        {
            direction = Vector2Int.down;
        }

        boardManager.MovePiece(this, direction);
    }

    public void OnNotify(EventKey key)
    {
        switch (key)
        {
            case EventKey.DotExplode:
                gameObject.SetActive(false);
                break;
            default: break;
        }
    }
}