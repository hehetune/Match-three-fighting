using Battle;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager GetInstance() => instance;

        public BattleUI BattleUI;

        private void Awake()
        {
            instance = this;
        }
    }
}