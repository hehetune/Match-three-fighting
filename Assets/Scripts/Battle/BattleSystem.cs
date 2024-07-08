using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UI;
using UnityEngine;

namespace Battle
{
    public class BattleSystem : MonoBehaviour
    {
        private static BattleSystem instance;

        public static BattleSystem GetInstance()
        {
            return instance;
        }

        [SerializeField] private Transform pfCharacterBattle;

        private CharacterBattle playerCharacterBattle;
        private CharacterBattle enemyCharacterBattle;
        private CharacterBattle activeCharacterBattle;
        private State state;
        private int bonusRounds = 0;

        private Coroutine decreaseEnergyCoroutine;

        private enum State
        {
            WaitingForPlayer,
            Busy,
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            playerCharacterBattle = SpawnCharacter(true);
            enemyCharacterBattle = SpawnCharacter(false);

            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }

        public void Reset()
        {
            playerCharacterBattle.Reset();
            enemyCharacterBattle.Reset();
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }

        private void StartDecreasePlayerEnergy()
        {
            if(decreaseEnergyCoroutine!=null) StopCoroutine(decreaseEnergyCoroutine);
            decreaseEnergyCoroutine = StartCoroutine(DecreaseEnergyCoroutine());
        }

        public bool CanPlayerAction()
        {
            // return activeCharacterBattle == playerCharacterBattle && state != State.Busy;
            // temp
            return state != State.Busy;
        }

        private IEnumerator DecreaseEnergyCoroutine()
        {
            while (activeCharacterBattle != null)
            {
                activeCharacterBattle.DecreaseEnergyByOneUnit();
                yield return 1f.Wait();
            }
        }

        public void PlayerPerformedTurn()
        {
            state = State.Busy;
            if(decreaseEnergyCoroutine!=null) StopCoroutine(decreaseEnergyCoroutine);
        }

        public void PerformPlayerTurn(MatchResult result)
        {
            PerformPlayerAction(result, activeCharacterBattle,
                activeCharacterBattle == playerCharacterBattle ? enemyCharacterBattle : playerCharacterBattle);
        }

        private Queue<Action> _playerActionsCache = new();

        private void ExecuteActionQueue()
        {
            if (_playerActionsCache.Count > 0)
            {
                var action = _playerActionsCache.Dequeue();
                action();
            }
            else
            {
                ChooseNextActiveCharacter();
            }
        }

        public void PerformPlayerAction(MatchResult result, CharacterBattle mainCharacter,
            CharacterBattle targetCharacter)
        {
            _playerActionsCache = new Queue<Action>();
            if (result.numberSword > 0)
            {
                _playerActionsCache.Enqueue(() =>
                    mainCharacter.Attack(result.numberSword, targetCharacter, ExecuteActionQueue));
            }

            if (result.numberHp > 0)
            {
                _playerActionsCache.Enqueue(() => mainCharacter.RestoreHp(result.numberHp, ExecuteActionQueue));
            }

            if (result.numberMana > 0)
            {
                _playerActionsCache.Enqueue(() => mainCharacter.RestoreMana(result.numberMana, ExecuteActionQueue));
            }

            if (result.numberEnergy > 0)
            {
                _playerActionsCache.Enqueue(() =>
                    mainCharacter.RestoreEnergy(result.numberEnergy, ExecuteActionQueue));
            }

            ExecuteActionQueue();
        }

        private CharacterBattle SpawnCharacter(bool isPlayerTeam)
        {
            var position = isPlayerTeam ? new Vector3(-6.75f, -3) : new Vector3(+6.75f, -3);

            Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
            CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
            characterBattle.Setup(isPlayerTeam);

            return characterBattle;
        }

        private void SetActiveCharacterBattle(CharacterBattle characterBattle)
        {
            activeCharacterBattle = characterBattle;
            StartDecreasePlayerEnergy();
        }

        private void ChooseNextActiveCharacter()
        {
            if (TestBattleOver())
            {
                return;
            }

            if (bonusRounds > 0)
            {
                bonusRounds--;
                if (activeCharacterBattle == enemyCharacterBattle)
                {
                    // TODO: enemy AI action

                    // temp
                    state = State.WaitingForPlayer;
                }
                else state = State.WaitingForPlayer;
            }
            else if (activeCharacterBattle == playerCharacterBattle)
            {
                SetActiveCharacterBattle(enemyCharacterBattle);
                // TODO: enemy AI action
                // state = State.Busy;
                UIManager.GetInstance().BattleUI.UpdateBorderColor(false);
                // temp
                state = State.WaitingForPlayer;
            }
            else
            {
                SetActiveCharacterBattle(playerCharacterBattle);
                UIManager.GetInstance().BattleUI.UpdateBorderColor(true);
                state = State.WaitingForPlayer;
            }
        }

        private bool TestBattleOver()
        {
            if (playerCharacterBattle.IsDead())
            {
                // Player dead, enemy wins
                BattleOverWindow.Show_Static("Enemy Wins!");
                return true;
            }

            if (enemyCharacterBattle.IsDead())
            {
                // Enemy dead, player wins
                BattleOverWindow.Show_Static("Player Wins!");
                return true;
            }

            return false;
        }
    }
}