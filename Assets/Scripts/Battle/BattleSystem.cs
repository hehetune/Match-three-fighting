using System;
using System.Collections.Generic;
using Character;
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

        public BattleUI battleUI;

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
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }

        public bool CanPlayerAction()
        {
            // return activeCharacterBattle == playerCharacterBattle && state != State.Busy;
            // temp
            return state != State.Busy;
        }

        public void SetStateBusy() => state = State.Busy;

        public void PerformPlayerTurn(MatchResult result)
        {
            PerformPlayerAction(result, activeCharacterBattle,
                activeCharacterBattle == playerCharacterBattle ? enemyCharacterBattle : playerCharacterBattle);
        }

        public void PerformPlayerAction(MatchResult result, CharacterBattle mainCharacter,
            CharacterBattle targetCharacter)
        {
            void ExecuteActionQueue(Queue<Action> actions)
            {
                if (actions.Count > 0)
                {
                    var action = actions.Dequeue();
                    action();
                }
                else
                {
                    ChooseNextActiveCharacter();
                }
            }

            Queue<Action> actions = new Queue<Action>();

            if (result.numberSword > 0)
            {
                actions.Enqueue(() =>
                    mainCharacter.Attack(result.numberSword, targetCharacter, () => ExecuteActionQueue(actions)));
            }

            if (result.numberHp > 0)
            {
                actions.Enqueue(() => mainCharacter.RestoreHp(result.numberHp, () => ExecuteActionQueue(actions)));
            }

            if (result.numberMana > 0)
            {
                actions.Enqueue(() => mainCharacter.RestoreMana(result.numberMana, () => ExecuteActionQueue(actions)));
            }

            if (result.numberEnergy > 0)
            {
                actions.Enqueue(() =>
                    mainCharacter.RestoreEnergy(result.numberEnergy, () => ExecuteActionQueue(actions)));
            }

            ExecuteActionQueue(actions);
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
                
                // temp
                state = State.WaitingForPlayer;
            }
            else
            {
                SetActiveCharacterBattle(playerCharacterBattle);
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