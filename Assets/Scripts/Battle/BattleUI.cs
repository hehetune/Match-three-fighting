using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleUI : MonoBehaviour
    {
        public List<SpriteRenderer> boardBorders = new();

        public Color playerColor;
        public Color enemyColor;

        public CharacterStatusUI playerUI;
        public CharacterStatusUI enemyUI;

        public void UpdateBorderColor(bool isPlayerTurn)
        {
            foreach (var border in boardBorders)
            {
                border.color = isPlayerTurn ? playerColor : enemyColor;
            }
        }
    }
}