using UnityEngine;
using UnityEngine.UI;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class GameModeSelectionView : MonoBehaviour
    {
        [SerializeField] private Button checkersButton;
        [SerializeField] private Button chessButton;

        public void Show(BattleController controller)
        {
            gameObject.SetActive(true);

            if (checkersButton != null)
            {
                checkersButton.onClick.RemoveAllListeners();
                checkersButton.onClick.AddListener(controller.StartCheckers);
            }

            if (chessButton != null)
            {
                chessButton.onClick.RemoveAllListeners();
                chessButton.onClick.AddListener(controller.StartChess);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
