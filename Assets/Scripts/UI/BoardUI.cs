using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class BoardUI : MonoBehaviour
    {
        public List<SpriteRenderer> boardBorders = new();

        public Color playerColor;
        public Color enemyColor;

        public void UpdateBorderColor(bool isPlayerTurn)
        {
            foreach (var border in boardBorders)
            {
                border.color = isPlayerTurn ? playerColor : enemyColor;
            }
        }
    }
}