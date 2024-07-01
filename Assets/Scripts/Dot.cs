using System.Collections;
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

public class Dot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IObserver
{
    [Header("Board")] public BoardManager boardManager;

    [Header("Interact")] private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;

    [Header("Swipe")] public float swipeAngle;
    public float swipeResist = .5f;

    [Header("Node")] public Node currentNode;

    [Header("Dot properties")] public DotType dotType;

    private Image _image;

    private RectTransform _rect;
    public RectTransform RectTransform
    {
        get
        {
            if (!_rect) _rect = gameObject.GetComponent<RectTransform>();
            return _rect;
        }
    }

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
        if (!_image) _image = gameObject.GetComponent<Image>();
        _image.sprite = ResourceUtil.Ins.GetSpriteByDotType(this.dotType);
    }

    // public bool isMatch = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnMouseDown1");
        if (GameManager.Ins.CurrentState == GameState.Move)
        {
            Debug.Log("OnMouseDown2");
            _firstTouchPosition = Input.mousePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.Ins.CurrentState == GameState.Move)
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
            Debug.Log("GameManager.Ins.CurrentState = GameState.Wait;");
            GameManager.Ins.CurrentState = GameState.Wait;
            swipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y,
                _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;

            HandleSwipe();
        }
        else
        {
            GameManager.Ins.CurrentState = GameState.Move;
            Debug.Log("GameManager.Ins.CurrentState = GameState.Move;");
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
        Debug.Log("Dot Start Move at " + Time.time + " , local Position: "+ _rect.position);
        
        Vector3 targetPosition = currentNode.RectTransform.position;
        while (Mathf.Abs(RectTransform.position.x - targetPosition.x) > 0.1f || Mathf.Abs(RectTransform.position.y - targetPosition.y) > 0.1f)
        {
            RectTransform.position = Vector3.Lerp(RectTransform.position, targetPosition, 0.2f);
            yield return null;
        }

        RectTransform.position = targetPosition;
        boardManager.OnDotStopMoving();
        Debug.Log("Dot Stop Move at " + Time.time);
    }

    public void UpdatePosition()
    {
        if (RectTransform.position == currentNode.RectTransform.position) return;
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