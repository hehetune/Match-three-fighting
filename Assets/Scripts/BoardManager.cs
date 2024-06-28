using System.Collections;
using System.Collections.Generic;
using Patterns.ObserverPattern;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")] public int width;
    public int height;

    [Header("Tiles")] public Dot[,] AllDots;

    private int _dotsMoving = 0;

    public void MovePiece(Dot dot, Vector2Int direction)
    {
        StartCoroutine(MovePieceCoroutine(dot, direction));
    }

    private IEnumerator MovePieceCoroutine(Dot currentDot, Vector2Int direction)
    {
        int col = currentDot.col;
        int row = currentDot.row;

        if (!IsValidMove(row, col, direction)) yield break;

        Dot otherDot = AllDots[col + direction.x, row + direction.y];

        if (otherDot == null) yield break;

        SwapDots(currentDot, otherDot);

        yield return new WaitUntil(() => _dotsMoving == 0);

        bool haveMatch = CheckMatchAtDot(currentDot);
        haveMatch = CheckMatchAtDot(otherDot) || haveMatch;

        if (haveMatch)
        {
            Subject.Notify(EventKey.DotExplode);
            // GameManager.Instance.CurrentState = GameState.Move;
        }
        else
        {
            SwapDots(currentDot, otherDot);
            yield return new WaitUntil(() => _dotsMoving == 0);
            GameManager.Instance.CurrentState = GameState.Move;
        }
    }

    private bool IsValidMove(int row, int col, Vector2Int direction)
    {
        return !(direction == Vector2Int.up && row == 0) &&
               !(direction == Vector2Int.down && row == height - 1) &&
               !(direction == Vector2Int.left && col == 0) &&
               !(direction == Vector2Int.right && col == width - 1);
    }

    private void SwapDots(Dot dot1, Dot dot2)
    {
        dot1.MoveToNode(dot2.transform.position);
        dot2.MoveToNode(dot1.transform.position);
        _dotsMoving++;
    }

    public void OnDotStopMoving() => _dotsMoving--;
    public void OnDotStartMoving() => _dotsMoving++;

    private bool CheckMatchAtDot(Dot dot)
    {
        if (dot == null)
            return false;

        int col = dot.col;
        int row = dot.row;
        DotType dotType = dot.dotType; // Replace GemType with your actual enum or type for gem types
        List<Dot> cacheDots = new List<Dot>();

        // Check horizontally
        int matchCount = 1;
        bool haveMatch = false;
        // Check to the left
        for (int c = col - 1;
             c >= 0 && AllDots[c, row] != null && AllDots[c, row].dotType == dotType;
             c--)
        {
            matchCount++;
            if (matchCount < 3)
            {
                cacheDots.Add(AllDots[c, row]);
            }
            else AllDots[c, row].isMatch = true;
        }

        // Check to the right
        for (int c = col + 1;
             c < width && AllDots[c, row] != null && AllDots[c, row].dotType == dotType;
             c++)
        {
            matchCount++;
            if (matchCount < 3)
            {
                cacheDots.Add(AllDots[c, row]);
            }
            else AllDots[c, row].isMatch = true;
        }

        if (matchCount >= 3)
        {
            foreach (var cacheDot in cacheDots)
            {
                cacheDot.isMatch = true;
            }

            haveMatch = true;
        }
        cacheDots.Clear();

        // Check vertically
        matchCount = 1;
        // Check below
        for (int r = row - 1;
             r >= 0 && AllDots[col, r] != null && AllDots[col, r].dotType == dotType;
             r--)
        {
            matchCount++;
            if (matchCount < 3)
            {
                cacheDots.Add(AllDots[col, r]);
            }
            else AllDots[col, r].isMatch = true;
        }

        // Check above
        for (int r = row + 1;
             r < height && AllDots[col, r] != null && AllDots[col, r].dotType == dotType;
             r++)
        {
            matchCount++;
            if (matchCount < 3)
            {
                cacheDots.Add(AllDots[col, r]);
            }
            else AllDots[col, r].isMatch = true;
        }

        if (matchCount >= 3)
        {
            foreach (var cacheDot in cacheDots)
            {
                cacheDot.isMatch = true;
            }
            haveMatch = true;
        }
        
        

        return haveMatch;
    }
}