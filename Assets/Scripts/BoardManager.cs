using System.Collections;
using System.Collections.Generic;
using Battle;
using Patterns.ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum MatchType
{
    Four = 0,
    Five = 1,
    L = 2,
    T = 3,
}

public class MatchResult
{
    public int numberHp = 0;
    public int numberSword = 0;
    public int numberMana = 0;
    public int numberEnergy = 0;
    public int numberGold = 0;
    public int numberExp = 0;

    public Dictionary<MatchType, int> matchTypesDic = new();
    public int bonusRound = 0;
}

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")] public int width;
    public int height;

    [Header("Nodes")] public List<Node> allNodes = new();

    private Node GetNode(int col, int row)
    {
        return allNodes[row * width + col];
    }

    [Header("Prefabs")] public Prefab dotPrefab;

    //track number of moving dots
    [SerializeField] private int dotsMoving = 0;

    public Prefab smokePrefab;

    private readonly HashSet<Dot> _matchDots = new();
    private int[] _dotsExplodeInColumns;

    private Dot _firstDot;
    private Dot _secondDot;

    private bool _movePerformed = false;

    private readonly List<DotType> _possibleDots = new();

    //cache for result
    private MatchResult cacheResult = new();

    private void Awake()
    {
        _dotsExplodeInColumns = new int[width];
        FillNodeIndexes();
    }

    public void Initialize()
    {
        SpawnDots();
        dotsMoving = 0;
    }

    private void FillNodeIndexes()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].col = i % width;
            allNodes[i].row = i / height;
        }
    }

    private void SpawnDots()
    {
        foreach (var node in allNodes)
        {
            if (node.currentDot)
                node.currentDot.gameObject.GetComponent<PoolObject>().ReturnToPool();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Clear the list and add all dot types
                _possibleDots.Clear();
                _possibleDots.AddRange((DotType[])System.Enum.GetValues(typeof(DotType)));

                // Remove the same type as the left neighbor
                if (x > 1 && GetNode(x - 1, y).currentDot.dotType == GetNode(x - 2, y).currentDot.dotType)
                {
                    _possibleDots.Remove(GetNode(x - 1, y).currentDot.dotType);
                }

                // Remove the same type as the bottom neighbor
                if (y > 1 && GetNode(x, y - 1).currentDot.dotType == GetNode(x, y - 2).currentDot.dotType)
                {
                    _possibleDots.Remove(GetNode(x, y - 1).currentDot.dotType);
                }

                Node curNode = GetNode(x, y);

                // Choose a random dot from the possible dots
                DotType chosenDotType = _possibleDots[Random.Range(0, _possibleDots.Count)];

                // Instantiate the dot prefab at the correct position
                PoolManager.Get<PoolObject>(dotPrefab, out var dotGo);
                dotGo.transform.SetParent(curNode.transform, false);
                Dot curDot = dotGo.GetComponent<Dot>();
                curDot.transform.position = curNode.transform.position;
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

        if (!IsValidMove(col, row, direction)) return;

        Node secondNode = GetNode(col + direction.x, row - direction.y);
        _secondDot = secondNode.currentDot;

        if (_secondDot == null) return;

        BattleSystem.GetInstance().PlayerPerformedTurn();

        SwapDotsAndUpdatePosition(_firstDot, _secondDot);

        CheckMatchAtNode(_firstDot.currentNode);
        CheckMatchAtNode(_secondDot.currentNode);
    }

    private void OnAllDotsStopMoving()
    {
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
            BattleSystem.GetInstance().PerformPlayerTurn(cacheResult);
            cacheResult = new();
        }
    }

    private void ResolveMatches()
    {
        if (_matchDots.Count == 0) return;

        StartCoroutine(ResolveMatchesCoroutine());
    }

    IEnumerator ResolveMatchesCoroutine()
    {
        foreach (var dot in _matchDots)
        {
            PoolManager.Get<PoolObject>(smokePrefab, out var smokeGo);
            smokeGo.transform.position = dot.transform.position;
            smokeGo.GetComponent<PoolObject>().ReturnToPoolByLifeTime(0.5f);
        }

        yield return new WaitForSeconds(0.2f);

        //variables for calculate match result
        int numberHp = 0, numberSword = 0, numberMana = 0, numberEnergy = 0, numberGold = 0, numberExp = 0;

        for (int curCol = 0; curCol < width; curCol++)
        {
            int curRow = height - 1;
            while (curRow >= 0)
            {
                Node curNode = GetNode(curCol, curRow);
                Dot curDot = curNode.currentDot;

                if (_matchDots.Contains(curDot))
                {
                    switch (curDot.dotType)
                    {
                        case DotType.Hp:
                            numberHp++;
                            break;
                        case DotType.Sword:
                            numberSword++;
                            break;
                        case DotType.Mana:
                            numberMana++;
                            break;
                        case DotType.Energy:
                            numberEnergy++;
                            break;
                        case DotType.Gold:
                            numberGold++;
                            break;
                        case DotType.Exp:
                            numberExp++;
                            break;
                        default: break;
                    }

                    _dotsExplodeInColumns[curCol]++;
                }
                else if (_dotsExplodeInColumns[curCol] > 0)
                {
                    Node targetNode = GetNode(curCol, curRow + _dotsExplodeInColumns[curCol]);
                    SwapDots(curDot, targetNode.currentDot);
                }

                curRow--;
            }
        }

        cacheResult.numberHp += numberHp;
        cacheResult.numberSword += numberSword;
        cacheResult.numberMana += numberMana;
        cacheResult.numberEnergy += numberEnergy;
        cacheResult.numberGold += numberGold;
        cacheResult.numberExp += numberExp;

        //shuffle new dots
        _possibleDots.Clear();
        _possibleDots.AddRange((DotType[])System.Enum.GetValues(typeof(DotType)));

        foreach (var dot in _matchDots)
        {
            dot.SetDotType(_possibleDots[Random.Range(0, _possibleDots.Count)]);
            dot.transform.position = dot.currentNode.transform.position +
                                     Vector3.up * _dotsExplodeInColumns[dot.currentNode.col];
        }

        Subject.Notify(EventKey.DotUpdatePosition);
        _matchDots.Clear();
        for (int i = 0; i < width; i++)
        {
            _dotsExplodeInColumns[i] = 0;
        }

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
        if (dotsMoving == 1)
        {
            OnAllDotsStopMoving();
        }

        dotsMoving--;
    }

    public void OnDotStartMoving() => dotsMoving += 1;

    private int numberOfCheck = 0;

    private void CheckMatchAtAllNode()
    {
        numberOfCheck = 0;
        foreach (var node in allNodes)
        {
            CheckMatchAtNode(node);
        }

        Debug.Log("Number of check: " + numberOfCheck);
    }

    private int _maxTilesOnLine = 0;

    private void CheckMatchAtNode(Node node)
    {
        if (node == null || _matchDots.Contains(node.currentDot))
            return;

        numberOfCheck++;
        DotType dotType = node.currentDot.dotType;
        List<Dot> cacheDots = new List<Dot> { node.currentDot };

        int left = CheckDirection(node, dotType, 0, -1, cacheDots);
        int right = CheckDirection(node, dotType, 0, 1, cacheDots) - 1;
        int horizontalCount = left + right;

        if (horizontalCount >= 3)
        {
            _maxTilesOnLine = horizontalCount;
            _matchDots.AddRange(cacheDots);
        }

        cacheDots.Clear();
        cacheDots.Add(node.currentDot);

        int below = CheckDirection(node, dotType, -1, 0, cacheDots);
        int above = CheckDirection(node, dotType, 1, 0, cacheDots) - 1;
        int verticalCount = below + above;

        if (verticalCount >= 3)
        {
            _maxTilesOnLine = Mathf.Max(_maxTilesOnLine, verticalCount);
            _matchDots.AddRange(cacheDots);
        }

        if (horizontalCount >= 4 || verticalCount >= 4)
        {
            cacheResult.bonusRound++;
        }

        if (horizontalCount >= 3 && verticalCount >= 3)
        {
            if ((left > 0 && right > 0) || (below > 0 && above > 0))
            {
                if (!cacheResult.matchTypesDic.TryAdd(MatchType.T, 1)) cacheResult.matchTypesDic[MatchType.T]++;
            }
            else
            {
                if (!cacheResult.matchTypesDic.TryAdd(MatchType.L, 1)) cacheResult.matchTypesDic[MatchType.L]++;
            }
        }

        if (horizontalCount >= 5 || verticalCount >= 5)
        {
            if (!cacheResult.matchTypesDic.TryAdd(MatchType.Five, 1)) cacheResult.matchTypesDic[MatchType.Five]++;
        }

        else if (horizontalCount >= 4 || verticalCount >= 4)
        {
            if (!cacheResult.matchTypesDic.TryAdd(MatchType.Four, 1)) cacheResult.matchTypesDic[MatchType.Four]++;
        }
    }

    private int CheckDirection(Node node, DotType dotType, int rowOffset, int colOffset, List<Dot> cacheDots)
    {
        int matchCount = 1;
        int row = node.row + rowOffset;
        int col = node.col + colOffset;

        while (IsValidPosition(row, col))
        {
            Node currentNode = GetNode(col, row);
            if (currentNode.currentDot != null && currentNode.currentDot.dotType == dotType)
            {
                matchCount++;
                cacheDots.Add(currentNode.currentDot);
            }
            else
            {
                break;
            }

            row += rowOffset;
            col += colOffset;
        }

        return matchCount;
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < height && col >= 0 && col < width;
    }
}