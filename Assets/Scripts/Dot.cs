using System.Collections;
using Battle;
using Patterns.ObserverPattern;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum DotType
{
    Sword = 0,
    Hp = 1,
    Mana = 2,
    Energy = 3,
    Gold = 4,
    Exp = 5,
}

public class Dot : MonoBehaviour, IObserver
{
    [Header("Board")] public BoardManager boardManager;

    [Header("Interact")] private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;

    [Header("Swipe")] public float swipeAngle;
    public float swipeResist = .5f;

    [Header("Node")] public Node currentNode;

    [Header("Dot properties")] public DotType dotType;
    public float movingSpeed = 1;

    private SpriteRenderer _image;

    // private RectTransform _rect;
    // public RectTransform RectTransform
    // {
    //     get
    //     {
    //         if (!_rect) _rect = gameObject.GetComponent<RectTransform>();
    //         return _rect;
    //     }
    // }

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
        Subject.Register(this, EventKey.DotUpdatePosition);
    }
        
    private void UnregisterEvents()
    {
        Subject.Unregister(this, EventKey.DotUpdatePosition);
    }


    public void SetDotType(DotType dotType)
    {
        this.dotType = dotType;
        if (!_image) _image = gameObject.GetComponent<SpriteRenderer>();
        _image.sprite = ResourceUtil.Ins.GetSpriteByDotType(dotType);
    }

    // public bool isMatch = false;
    public void OnMouseDown()
    {
        Debug.Log("on mouse down");
        if (BattleSystem.GetInstance().CanPlayerAction())
        {
            _firstTouchPosition = Input.mousePosition;
        }
    }

    public void OnMouseUp()
    {
        Debug.Log("on mouse up");

        if (BattleSystem.GetInstance().CanPlayerAction())
        {
            _finalTouchPosition = Input.mousePosition;
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > swipeResist ||
            Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > swipeResist)
        {
            
            swipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y,
                _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;

            HandleSwipe();
        }
    }

    // public void MoveToNode(Node targetNode)
    // {
    //     ChangeNode(targetNode);        
    //     UpdatePosition();
    // }

    public void ChangeNode(Node targetNode)
    {
        currentNode = targetNode;
        targetNode.currentDot = this;
        transform.SetParent(targetNode.transform, true);
    }

    private IEnumerator MoveToPositionCoroutine()
    {
        boardManager.OnDotStartMoving();
    
        Vector3 targetPosition = currentNode.transform.position;
        float step = movingSpeed * 0.2f;
    
        while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.005f || Mathf.Abs(transform.position.y - targetPosition.y) > 0.005f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);
            yield return null;
        }

        transform.position = targetPosition;
        boardManager.OnDotStopMoving();
    }

    public void UpdatePosition()
    {
        if (transform.position == currentNode.transform.position) return;
        StartCoroutine(MoveToPositionCoroutine());
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
            case EventKey.DotUpdatePosition:
                UpdatePosition();
                break;
            default: break;
        }
    }
}