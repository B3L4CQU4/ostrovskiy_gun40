using System;
using TacticalPrototype.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class PromotionChoiceView : MonoBehaviour, IPromotionChoiceView
    {
        [SerializeField] private Button queenButton;
        [SerializeField] private Button rookButton;
        [SerializeField] private Button bishopButton;
        [SerializeField] private Button knightButton;

        private Action<UnitType> onChoice;

        public void Show(Action<UnitType> choiceCallback)
        {
            onChoice = choiceCallback;
            gameObject.SetActive(true);

            Bind(queenButton, UnitType.Queen);
            Bind(rookButton, UnitType.Rook);
            Bind(bishopButton, UnitType.Bishop);
            Bind(knightButton, UnitType.Knight);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Bind(Button button, UnitType choice)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                Action<UnitType> callback = onChoice;
                onChoice = null;
                Hide();

                if (callback != null)
                {
                    callback.Invoke(choice);
                }
            });
        }
    }
}
