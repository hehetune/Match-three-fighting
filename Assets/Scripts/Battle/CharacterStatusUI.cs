using System;
using System.Collections;
using UI;
using UnityEngine;

namespace Battle
{
    public class CharacterStatusUI : MonoBehaviour
    {
        public SpriteFillController hpBar;
        public SpriteFillController manaBar;
        public SpriteFillController energyBar;

        private float currentHp = 1;
        private float currentMana = 0;
        private float currentEnergy = 1;
        private float targetHp = 1;
        private float targetMana = 0;
        private float targetEnergy = 1;

        private float MIN_DIFF = 0.01f;

        private Coroutine changeHpCoroutine;
        private Coroutine changeManaCoroutine;
        private Coroutine changeEnergyCoroutine;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            hpBar.Initialize();
            manaBar.Initialize();
            energyBar.Initialize();
            // hpBar.SetFill(currentHp);
            // manaBar.SetFill(currentMana);
            // energyBar.SetFill(currentEnergy);
        }

        public void OnHpChanged(float amount)
        {
            targetHp = amount;
            if(changeHpCoroutine != null) StopCoroutine(changeHpCoroutine);
            changeHpCoroutine = StartCoroutine(ChangeHpCoroutine());
        }
        
        public void OnManaChanged(float amount)
        {
            targetMana = amount;
            if(changeManaCoroutine != null) StopCoroutine(changeManaCoroutine);
            changeManaCoroutine = StartCoroutine(ChangeManaCoroutine());
        }
        
        public void OnEnergyChanged(float amount)
        {
            targetEnergy = amount;
            if(changeEnergyCoroutine != null) StopCoroutine(changeEnergyCoroutine);
            changeEnergyCoroutine = StartCoroutine(ChangeEnergyCoroutine());
        }

        IEnumerator ChangeHpCoroutine()
        {
            while (Mathf.Abs(currentHp - targetHp) > 0.01f)
            {
                currentHp += (currentHp > targetHp ? -1 : 1) * Time.deltaTime;
                hpBar.SetFill(currentHp);
                yield return null;
            }

            currentHp = targetHp;
            hpBar.SetFill(currentHp);
        }

        IEnumerator ChangeManaCoroutine()
        {
            while (Mathf.Abs(currentMana - targetMana) > 0.01f)
            {
                currentMana += (currentMana > targetMana ? -1 : 1) * Time.deltaTime;
                manaBar.SetFill(currentMana);
                yield return null;
            }

            currentMana = targetMana;
            manaBar.SetFill(currentMana);
        }
        
        IEnumerator ChangeEnergyCoroutine()
        {
            while (Mathf.Abs(currentEnergy - targetEnergy) > 0.01f)
            {
                currentEnergy += (currentEnergy > targetEnergy ? -1 : 1) * Time.deltaTime;
                energyBar.SetFill(currentEnergy);
                yield return null;
            }

            currentEnergy = targetEnergy;
            energyBar.SetFill(currentEnergy);
        }
    }
}