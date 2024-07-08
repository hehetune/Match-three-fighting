using System;
using System.Collections;
using Battle;
using CodeMonkey.Utils;
using UI;
using UnityEngine;

namespace Character
{
    public class CharacterBattle : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Sliding,
            Busy,
        }

        private CharacterBase characterBase;
        private State state;
        private Vector3 slideTargetPosition;
        private Action onSlideComplete;
        public bool isPlayerTeam;
        private CharacterHealth characterHealth;
        private CharacterMana characterMana;
        private CharacterEnergy characterEnergy;

        [SerializeField] private int attackBase;
        [SerializeField] private int hpRestoreBase;
        [SerializeField] private int manaRestoreBase;
        [SerializeField] private int energyRestoreBase;

        [SerializeField] private CharacterSkill skill1;
        [SerializeField] private CharacterSkill skill2;
        [SerializeField] private CharacterSkill skill3;
        // private World_Bar healthBar;

        protected bool facingRight;

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            state = State.Idle;
            Reset();
        }

        public void Setup(bool isPlayerTeam)
        {
            this.facingRight = isPlayerTeam;
            characterBase.characterAnimator.SetFaceDirection(isPlayerTeam);
            this.isPlayerTeam = isPlayerTeam;
        }

        public void Reset()
        {
            characterHealth = new CharacterHealth(100);
            characterMana = new CharacterMana(90);
            characterEnergy = new CharacterEnergy(100);
            characterHealth.OnValueChanged += OnCharacterHealthChanged;
            characterMana.OnValueChanged += OnCharacterManaChanged;
            characterEnergy.OnValueChanged += OnCharacterEnergyChanged;
            characterHealth.Reset();
            characterMana.Reset();
            characterEnergy.Reset();
        }

        // private void Update()
        // {
        //     switch (state)
        //     {
        //         case State.Idle:
        //             break;
        //         case State.Busy:
        //             break;
        //         case State.Sliding:
        //             float slideSpeed = 10f;
        //             transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;
        //
        //             float reachedDistance = 1f;
        //             if (Vector3.Distance(GetPosition(), slideTargetPosition) < 0.01f)
        //             {
        //                 // Arrived at Slide Target Position
        //                 //transform.position = slideTargetPosition;
        //                 onSlideComplete();
        //             }
        //
        //             break;
        //     }
        // }

        private void OnCharacterHealthChanged(float value)
        {
            UIManager.GetInstance().BattleUI.UpdateHpBar(value, this.isPlayerTeam);
        }

        private void OnCharacterManaChanged(float value)
        {
            UIManager.GetInstance().BattleUI.UpdateManaBar(value, this.isPlayerTeam);
        }

        private void OnCharacterEnergyChanged(float value)
        {
            UIManager.GetInstance().BattleUI.UpdateEnergyBar(value, this.isPlayerTeam);
        }

        private void PlayAnimIdle()
        {
            characterBase.characterAnimator.PlayIdleAnimation();
        }

        public void UseSkill(CharacterSkill characterSkill)
        {
            if (!characterMana.Consume(characterSkill.skillType)) return;
            characterSkill.Execute();
        }

        public void RestoreMana(int gemsCount, Action onComplete)
        {
            characterBase.characterAnimator.PlayHealAnimation(() => {}, () =>
            {
                int amount = gemsCount * manaRestoreBase;
                TextPopup.Create(GetPosition(), "+" + amount + " mana", TextPopupType.RegenMana);
                Debug.Log((isPlayerTeam ? "player" : "enemy") +" restore " + amount + " mana");
                characterMana.Add(amount);
                onComplete();
            });
        }

        public void RestoreHp(int gemsCount, Action onComplete)
        {
            characterBase.characterAnimator.PlayHealAnimation(() => {}, () =>
            {
                int amount = gemsCount * hpRestoreBase;
                TextPopup.Create(GetPosition(), "+" + amount + " hp", TextPopupType.HealHp);
                Debug.Log((isPlayerTeam ? "player" : "enemy") +" restore " + amount + " hp");
                characterHealth.Heal(amount);
                onComplete();
            });
        }

        public void RestoreEnergy(int gemsCount, Action onComplete)
        {
            characterBase.characterAnimator.PlayHealAnimation(() => {}, () =>
            {
                int amount = gemsCount * energyRestoreBase;
                TextPopup.Create(GetPosition(), "+" + amount + " energy", TextPopupType.RegenEnergy);
                Debug.Log((isPlayerTeam ? "player" : "enemy") +" restore " + amount + " energy");
                characterEnergy.Add(amount);
                onComplete();
            });
        }

        public void Attack(int gemsCount, CharacterBattle targetCharacterBattle, Action onComplete)
        {
            Vector3 startingPosition = GetPosition();
            // Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (startingPosition - targetCharacterBattle.GetPosition()).normalized * 10f;
            Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() +
                                          (startingPosition - targetCharacterBattle.GetPosition()).normalized * 2f;
            // Vector3 slideTargetPosition = targetCharacterBattle.GetPosition();

            // Slide to Target
            SlideToPosition(slideTargetPosition, () =>
            {
                // Arrived at Target, attack him
                state = State.Busy;
                Vector3 attackDir = slideTargetPosition.normalized;
                characterBase.PlayAnimAttack(attackDir, () =>
                {
                    // Target hit
                    int damageAmount = attackBase * gemsCount;
                    targetCharacterBattle.Damage(this, damageAmount);
                }, () =>
                {
                    ReverseFacingDirection(!facingRight);
                    state = State.Idle;
                    // Attack completed, slide back
                    SlideToPosition(startingPosition, () =>
                    {
                        ReverseFacingDirection(facingRight);
                        // Slide back completed, back to idle
                        state = State.Idle;
                        characterBase.characterAnimator.PlayIdleAnimation();
                        onComplete();
                    });
                });
            });
        }

        IEnumerator SlideCoroutine()
        {
            float slideSpeed = 50f;
            while (Mathf.Abs(transform.position.x - slideTargetPosition.x) > 0.01f)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, slideTargetPosition, slideSpeed * Time.deltaTime);
                yield return null;
            }

            // transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

            // if (Vector3.Distance(GetPosition(), slideTargetPosition) < 0.01f)
            // {
                // Arrived at Slide Target Position
                transform.position = slideTargetPosition;
                onSlideComplete();
            // }
        }

        private void ReverseFacingDirection(bool direction)
        {
            characterBase.characterAnimator.SetFaceDirection(direction);
        }

        public bool IsDead()
        {
            Debug.Log("Character battle dead!");
            return characterHealth.IsDead();
        }

        public void Damage(CharacterBattle attacker, int damageAmount)
        {
            characterHealth.Damage(damageAmount);
            //CodeMonkey.CMDebug.TextPopup("Hit " + healthSystem.GetHealthAmount(), GetPosition());
            // Vector3 dirFromAttacker = (GetPosition() - attacker.GetPosition()).normalized;

            TextPopup.Create(GetPosition(), damageAmount.ToString(), TextPopupType.Damage);
            characterBase.characterAnimator.PlayHurtAnimation(() => { }, () => { });
            // characterBase.SetColorTint(new Color(1, 0, 0, 1f));
            // Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);

            // UtilsClass.ShakeCamera(1f, .1f);

            if (characterHealth.IsDead())
            {
                // Died
                characterBase.characterAnimator.PlayDieAnimation(() => { }, () => { });
            }
        }

        private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
        {
            this.slideTargetPosition = slideTargetPosition;
            this.onSlideComplete = onSlideComplete;
            state = State.Sliding;
            characterBase.characterAnimator.PlaySlideAnimation(() => { }, () => {});
            StartCoroutine(SlideCoroutine());
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void DecreaseEnergyByOneUnit()
        {
            characterEnergy.Subtract(1);
        }
    }
}