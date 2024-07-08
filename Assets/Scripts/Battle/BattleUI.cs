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

        public void UpdateHpBar(float value, bool isPlayer)
        {
            if(isPlayer) playerUI.OnHpChanged(value);
            else enemyUI.OnHpChanged(value);
        }
        
        public void UpdateManaBar(float value, bool isPlayer)
        {
            if(isPlayer) playerUI.OnManaChanged(value);
            else enemyUI.OnManaChanged(value);
        }
        
        public void UpdateEnergyBar(float value, bool isPlayer)
        {
            if(isPlayer) playerUI.OnEnergyChanged(value);
            else enemyUI.OnEnergyChanged(value);
        }
    }
}