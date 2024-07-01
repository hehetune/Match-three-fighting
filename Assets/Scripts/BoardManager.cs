using System.Collections;
using System.Collections.Generic;
using Patterns.ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")] public int width;
    public int height;
    public float cellSize;

    [Header("Nodes")] public List<Node> AllNodes = new();

    private Node GetNode(int col, int row)
    {
        return AllNodes[row * width + col];
    }

    [Header("Prefabs")] public Prefab dotPrefab;

    //track number of moving dots
    [SerializeField] private int _dotsMoving = 0;

    // public int DotsMoving
    // {
    //     get => _dotsMoving;
    //     set
    //     {
    //         Debug.Log("Set DotsMoving: " + value);
    //         if (_dotsMoving > 0 && value == 0)
    //         {
    //             OnAllDotsStopMoving();
    //         }
    //
    //         _dotsMoving = value;
    //     }
    // }

    private HashSet<Dot> _matchDots = new();
    private int[] _dotsExplodeInColumns;

    private Dot _firstDot;
    private Dot _secondDot;

    private bool _movePerformed = false;

    private List<DotType> possibleDots = new();

    private void Awake()
    {
        _dotsExplodeInColumns = new int[width];
        FillNodeIndexes();
    }

    public void Initialize()
    {
        SpawnDots();
        _dotsMoving = 0;
    }

    private void FillNodeIndexes()
    {
        for (int i = 0; i < AllNodes.Count; i++)
        {
            AllNodes[i].col = i % width;
            AllNodes[i].row = i / height;
        }
    }

    public void SpawnDots()
    {
        foreach (var node in AllNodes)
        {
            if (node.currentDot)
                node.currentDot.gameObject.GetComponent<PoolObject>().ReturnToPool();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Clear the list and add all dot types
                possibleDots.Clear();
                possibleDots.AddRange((DotType[])System.Enum.GetValues(typeof(DotType)));

                // Remove the same type as the left neighbor
                if (x > 1 && GetNode(x - 1, y).currentDot.dotType == GetNode(x - 2, y).currentDot.dotType)
                {
                    possibleDots.Remove(GetNode(x - 1, y).currentDot.dotType);
                }

                // Remove the same type as the bottom neighbor
                if (y > 1 && GetNode(x, y - 1).currentDot.dotType == GetNode(x, y - 2).currentDot.dotType)
                {
                    possibleDots.Remove(GetNode(x, y - 1).currentDot.dotType);
                }

                Node curNode = GetNode(x, y);

                // Choose a random dot from the possible dots
                DotType chosenDotType = possibleDots[Random.Range(0, possibleDots.Count)];

                // Instantiate the dot prefab at the correct position
                PoolManager.Get<PoolObject>(dotPrefab, out var dotGo);
                dotGo.transform.SetParent(curNode.transform, false);
                Dot curDot = dotGo.GetComponent<Dot>();
                curDot.RectTransform.position = curNode.RectTransform.position;
                curDot.boardManager = this;
                curDot.SetDotType(chosenDotType);
                curDot.currentNode = curNode;
                curNode.currentDot = curDot;
            }
        }
    }

    public void MovePiece(Dot dot, Vector2Int direction)
    {
        Debug.Log("BoardManager::MovePiece at (" + dot.currentNode.col + "," + dot.currentNode.row + ") direction: " +
                  direction.x + ", " + direction.y);
        _movePerformed = true;
        _firstDot = dot;
        int col = _firstDot.currentNode.col;
        int row = _firstDot.currentNode.row;

        if (!IsValidMove(col, row, direction))
        {
            GameManager.Ins.CurrentState = GameState.Move;
            Debug.Log("GameManager.Ins.CurrentState = GameState.Move;");
            return;
        }

        Node secondNode = GetNode(col + direction.x, row - direction.y);
        _secondDot = secondNode.currentDot;

        if (_secondDot == null)
        {
            GameManager.Ins.CurrentState = GameState.Move;
            Debug.Log("GameManager.Ins.CurrentState = GameState.Move;");
            return;
        }

        SwapDotsAndUpdatePosition(_firstDot, _secondDot);

        CheckMatchAtNode(_firstDot.currentNode);
        CheckMatchAtNode(_secondDot.currentNode);
    }

    private void OnAllDotsStopMoving()
    {
        Debug.Log("BoardManager::OnAllDotsStopMoving");
        if (_matchDots.Count > 0)
        {
            ResolveMatches();
            _movePerformed = false;
        }
        else if (_movePerformed)
        {
            SwapDotsAndUpdatePosition(_firstDot, _secondDot);
            _movePerformed = false;
        }
        else
        {
            GameManager.Ins.CurrentState = GameState.Move;
            Debug.Log("GameManager.Ins.CurrentState = GameState.Move;");
        }
    }

    private void ResolveMatches()
    {
        if (_matchDots.Count == 0) return;
        foreach (var dot in _matchDots)
        {
            Debug.Log(dot.currentNode.col + ", " + dot.currentNode.row);
        }
        for (int curCol = 0; curCol < width; curCol++)
        {
            int curRow = height - 1;
            while (curRow >= 0 && curRow > _dotsExplodeInColumns[curCol] - 1)
            {
                Node curNode = GetNode(curCol, curRow);
                Dot curDot = curNode.currentDot;
                
                if (_matchDots.Contains(curDot))
                {
                    _dotsExplodeInColumns[curCol]++;
                    curDot.RectTransform.position += Vector3.up * 1000;
                }
                else if (_dotsExplodeInColumns[curCol] > 0)
                {
                    Node targetNode = GetNode(curCol, curRow + _dotsExplodeInColumns[curCol]);
                    SwapDots(curDot, targetNode.currentDot);
                }

                curRow--;
            }
        }
        
        //suffle new dots
        possibleDots.Clear();
        possibleDots.AddRange((DotType[])System.Enum.GetValues(typeof(DotType)));

        foreach (var dot in _matchDots)
        {
            dot.SetDotType(possibleDots[Random.Range(0, possibleDots.Count)]);
        }
        
        Subject.Notify(EventKey.DotUpdatePosition);
        _matchDots.Clear();
        CheckMatchAtAllNode();
    }

    private bool IsValidMove(int col, int row, Vector2Int direction)
    {
        return !(direction == Vector2Int.up && row == 0) &&
               !(direction == Vector2Int.down && row == height - 1) &&
               !(direction == Vector2Int.left && col == 0) &&
               !(direction == Vector2Int.right && col == width - 1);
    }

    private void SwapDots(Dot dot1, Dot dot2)
    {
        Node dot1Node = dot1.currentNode;
        dot1.ChangeNode(dot2.currentNode);
        dot2.ChangeNode(dot1Node);
    }
    
    private void SwapDotsAndUpdatePosition(Dot dot1, Dot dot2)
    {
        SwapDots(dot1, dot2);
        dot1.UpdatePosition();
        dot2.UpdatePosition();
    }

    public void OnDotStopMoving()
    {
        if (_dotsMoving == 1)
        {
            OnAllDotsStopMoving();
        }

        _dotsMoving--;
    }

    public void OnDotStartMoving() => _dotsMoving += 1;
    
    private int numberOfCheck = 0;

    private void CheckMatchAtAllNode()
    {
        numberOfCheck = 0;
        foreach (var node in AllNodes)
        {
            CheckMatchAtNode(node);
        }
        Debug.Log("Number of check: " + numberOfCheck);
    }

    private void CheckMatchAtNode(Node node)
    {
        if (node == null || _matchDots.Contains(node.currentDot))
            return;
        numberOfCheck++;
        int col = node.col;
        int row = node.row;
        DotType dotType = node.currentDot.dotType; // Replace GemType with your actual enum or type for gem types
        List<Dot> cacheDots = new List<Dot> { node.currentDot };

        // Check horizontally
        int matchCount = 1;
        // Check to the left
        for (int c = col - 1; c >= 0; c--)
        {
            Node leftNode = GetNode(c, row);
            if (leftNode.currentDot != null && leftNode.currentDot.dotType == dotType)
            {
                matchCount++;
                if (matchCount < 3)
                {
                    cacheDots.Add(leftNode.currentDot);
                }
                else
                {
                    _matchDots.Add(leftNode.currentDot);
                }
            }
            else
            {
                break;
            }
        }

        // Check to the right
        for (int c = col + 1; c < width; c++)
        {
            Node rightNode = GetNode(c, row);
            if (rightNode.currentDot != null && rightNode.currentDot.dotType == dotType)
            {
                matchCount++;
                if (matchCount < 3)
                {
                    cacheDots.Add(rightNode.currentDot);
                }
                else
                {
                    _matchDots.Add(rightNode.currentDot);
                }
            }
            else
            {
                break;
            }
        }

        if (matchCount >= 3)
        {
            foreach (var cacheDot in cacheDots)
            {
                _matchDots.Add(cacheDot);
            }
        }

        cacheDots.Clear();
        cacheDots.Add(node.currentDot);

        // Check vertically
        matchCount = 1;
        // Check below
        for (int r = row - 1; r >= 0; r--)
        {
            Node belowNode = GetNode(col, r);
            if (belowNode.currentDot != null && belowNode.currentDot.dotType == dotType)
            {
                matchCount++;
                if (matchCount < 3)
                {
                    cacheDots.Add(belowNode.currentDot);
                }
                else
                {
                    _matchDots.Add(belowNode.currentDot);
                }
            }
            else
            {
                break;
            }
        }

        // Check above
        for (int r = row + 1; r < height; r++)
        {
            Node aboveNode = GetNode(col, r);
            if (aboveNode.currentDot != null && aboveNode.currentDot.dotType == dotType)
            {
                matchCount++;
                if (matchCount < 3)
                {
                    cacheDots.Add(aboveNode.currentDot);
                }
                else
                {
                    _matchDots.Add(aboveNode.currentDot);
                }
            }
            else
            {
                break;
            }
        }

        if (matchCount >= 3)
        {
            foreach (var cacheDot in cacheDots)
            {
                _matchDots.Add(cacheDot);
            }
        }

        Debug.Log("BoardManager::CheckMatchAtDot Done check");
    }
}