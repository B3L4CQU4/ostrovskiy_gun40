using UnityEngine;
using UnityEngine.UI;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Text modeText;
        [SerializeField] private Text turnText;
        [SerializeField] private Text phaseText;
        [SerializeField] private Text statusText;

        public void SetTurn(GameKind gameKind, Team activeTeam, TurnPhase phase)
        {
            if (modeText != null)
            {
                modeText.text = "Mode: " + gameKind;
            }

            if (turnText != null)
            {
                turnText.text = "Turn: " + activeTeam;
            }

            if (phaseText != null)
            {
                phaseText.text = "Phase: " + phase;
            }
        }

        public void SetStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = status;
            }
        }
    }
}
