using UI;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        public BoardUI boardUI;

        public void UpdateUIByTurn(bool isPlayerTurn)
        {
            boardUI.UpdateBorderColor(isPlayerTurn);
        }
    }
}