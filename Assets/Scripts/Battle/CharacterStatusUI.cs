using System;
using UnityEngine;

namespace Battle
{
    public class CharacterStatusUI : MonoBehaviour
    {
        public Material hpBar;
        public Material manaBar;
        public Material energyBar;

        public void OnHpChange(float amount)
        {
            hpBar.SetFloat("_FillAmount", amount);
        }
    }
}