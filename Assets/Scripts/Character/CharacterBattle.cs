using System;
using Battle;
using CodeMonkey.Utils;
using UnityEngine;

namespace Character
{
    public class CharacterBattle : MonoBehaviour
    {
        private enum State {
            Idle,
            Sliding,
            Busy,
        }
        
        private CharacterBase characterBase;
        private State state;
        private Vector3 slideTargetPosition;
        private Action onSlideComplete;
        private bool isPlayerTeam;
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
        
        private void Awake() {
            characterBase = GetComponent<CharacterBase>();
            state = State.Idle;
        }
        
        public void Setup(bool isPlayerTeam) {
            this.isPlayerTeam = isPlayerTeam;
            characterHealth = new CharacterHealth(100);
            characterMana = new CharacterMana(90);
            characterEnergy = new CharacterEnergy(100);
            // healthBar = new World_Bar(transform, new Vector3(0, 10), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
            characterHealth.OnValueChanged += OnCharacterHealthChanged;
            characterMana.OnValueChanged += OnCharacterManaChanged;
            characterEnergy.OnValueChanged += OnCharacterEnergyChanged;

            PlayAnimIdle();
        }
        
        private void Update() {
            switch (state) {
                case State.Idle:
                    break;
                case State.Busy:
                    break;
                case State.Sliding:
                    float slideSpeed = 10f;
                    transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

                    float reachedDistance = 1f;
                    if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance) {
                        // Arrived at Slide Target Position
                        //transform.position = slideTargetPosition;
                        onSlideComplete();
                    }
                    break;
            }
        }
        
        private void OnCharacterHealthChanged(float value) {
            // BattleSystem.GetInstance().battleUI.
        }
        private void OnCharacterManaChanged(float value) {
            // BattleSystem.GetInstance().battleUI.
        }
        private void OnCharacterEnergyChanged(float value) {
            // BattleSystem.GetInstance().battleUI.
        }

        private void PlayAnimIdle() {
            characterBase.characterAnimator.PlayIdleAnimation();
        }

        public void UseSkill(CharacterSkill characterSkill)
        {
            if (!characterMana.Consume(characterSkill.skillType)) return;
            characterSkill.Execute();
        }
        
        public void RestoreMana(int gemsCount, Action onComplete)
        {
            DamagePopup.Create(GetPosition(), gemsCount, false);
            characterMana.Add(gemsCount * manaRestoreBase);
        }

        public void RestoreHp(int gemsCount, Action onComplete)
        {
            DamagePopup.Create(GetPosition(), gemsCount, false);
            characterHealth.Heal(gemsCount * hpRestoreBase);
        }

        public void RestoreEnergy(int gemsCount, Action onComplete)
        {
            DamagePopup.Create(GetPosition(), gemsCount, false);
            characterEnergy.Add(gemsCount * energyRestoreBase);
        }
        
        public void Attack(int gemsCount, CharacterBattle targetCharacterBattle, Action onComplete) {
            Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
            Vector3 startingPosition = GetPosition();

            // Slide to Target
            SlideToPosition(slideTargetPosition, () => {
                // Arrived at Target, attack him
                state = State.Busy;
                Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
                characterBase.PlayAnimAttack(attackDir, () => {
                    // Target hit
                    int damageAmount = attackBase * gemsCount;
                    targetCharacterBattle.Damage(this, damageAmount);
                }, () => {
                    // Attack completed, slide back
                    SlideToPosition(startingPosition, () => {
                        // Slide back completed, back to idle
                        state = State.Idle;
                        characterBase.characterAnimator.PlayIdleAnimation();
                        onComplete();
                    });
                });
            });
        }
        
        public bool IsDead() {
            return characterHealth.IsDead();
        }
        
        public void Damage(CharacterBattle attacker, int damageAmount) {
            characterHealth.Damage(damageAmount);
            //CodeMonkey.CMDebug.TextPopup("Hit " + healthSystem.GetHealthAmount(), GetPosition());
            // Vector3 dirFromAttacker = (GetPosition() - attacker.GetPosition()).normalized;

            DamagePopup.Create(GetPosition(), damageAmount, false);
            characterBase.SetColorTint(new Color(1, 0, 0, 1f));
            // Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);

            UtilsClass.ShakeCamera(1f, .1f);

            if (characterHealth.IsDead()) {
                // Died
                characterBase.characterAnimator.PlayDieAnimation(() => {}, () => {});
            }
        }
        
        private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete) {
            this.slideTargetPosition = slideTargetPosition;
            this.onSlideComplete = onSlideComplete;
            state = State.Sliding;
            characterBase.characterAnimator.PlaySlideAnimation(() => {}, onSlideComplete);
        }
        
        public Vector3 GetPosition() {
            return transform.position;
        }
    }
}