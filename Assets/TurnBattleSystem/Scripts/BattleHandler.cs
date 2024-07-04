/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour {

    private static BattleHandler instance;

    public static BattleHandler GetInstance() {
        return instance;
    }


    [SerializeField] private Transform pfCharacterBattle;
    public Texture2D playerSpritesheet;
    public Texture2D enemySpritesheet;

    private CharacterBattlee _playerCharacterBattlee;
    private CharacterBattlee _enemyCharacterBattlee;
    private CharacterBattlee _activeCharacterBattlee;
    private State state;

    private enum State {
        WaitingForPlayer,
        Busy,
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        _playerCharacterBattlee = SpawnCharacter(true);
        _enemyCharacterBattlee = SpawnCharacter(false);

        SetActiveCharacterBattle(_playerCharacterBattlee);
        state = State.WaitingForPlayer;
    }

    private void Update() {
        if (state == State.WaitingForPlayer) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = State.Busy;
                _playerCharacterBattlee.Attack(_enemyCharacterBattlee, () => {
                    ChooseNextActiveCharacter();
                });
            }
        }
    }

    private CharacterBattlee SpawnCharacter(bool isPlayerTeam) {
        Vector3 position;
        if (isPlayerTeam) {
            position = new Vector3(-50, 0);
        } else {
            position = new Vector3(+50, 0);
        }
        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattlee characterBattlee = characterTransform.GetComponent<CharacterBattlee>();
        characterBattlee.Setup(isPlayerTeam);

        return characterBattlee;
    }

    private void SetActiveCharacterBattle(CharacterBattlee characterBattlee) {
        if (_activeCharacterBattlee != null) {
            _activeCharacterBattlee.HideSelectionCircle();
        }

        _activeCharacterBattlee = characterBattlee;
        _activeCharacterBattlee.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter() {
        if (TestBattleOver()) {
            return;
        }

        if (_activeCharacterBattlee == _playerCharacterBattlee) {
            SetActiveCharacterBattle(_enemyCharacterBattlee);
            state = State.Busy;
            
            _enemyCharacterBattlee.Attack(_playerCharacterBattlee, () => {
                ChooseNextActiveCharacter();
            });
        } else {
            SetActiveCharacterBattle(_playerCharacterBattlee);
            state = State.WaitingForPlayer;
        }
    }

    private bool TestBattleOver() {
        if (_playerCharacterBattlee.IsDead()) {
            // Player dead, enemy wins
            //CodeMonkey.CMDebug.TextPopupMouse("Enemy Wins!");
            BattleOverWindow.Show_Static("Enemy Wins!");
            return true;
        }
        if (_enemyCharacterBattlee.IsDead()) {
            // Enemy dead, player wins
            //CodeMonkey.CMDebug.TextPopupMouse("Player Wins!");
            BattleOverWindow.Show_Static("Player Wins!");
            return true;
        }

        return false;
    }
}
