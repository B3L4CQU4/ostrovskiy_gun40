using System;
using System.Collections;
using TacticalPrototype.Controllers.Rules;
using TacticalPrototype.Units;
using UnityEngine;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveDuration = 0.18f;

        public bool IsBusy { get; private set; }

        public void PlayMove(MoveOption move, UnitType promotionChoice, Action onComplete)
        {
            StartCoroutine(PlayMoveRoutine(move, promotionChoice, onComplete));
        }

        private IEnumerator PlayMoveRoutine(MoveOption move, UnitType promotionChoice, Action onComplete)
        {
            IsBusy = true;

            Unit unit = move.Unit;
            Vector3 start = unit.transform.position;
            Vector3 end = move.Destination.UnitAnchor.position;

            foreach (Unit capture in move.Captures)
            {
                if (capture != null)
                {
                    capture.Capture();
                }
            }

            yield return AnimateTransform(unit.transform, start, end);
            unit.SetCell(move.Destination);
            unit.MarkMoved();

            if (move.Rook != null && move.RookDestination != null)
            {
                Vector3 rookStart = move.Rook.transform.position;
                Vector3 rookEnd = move.RookDestination.UnitAnchor.position;
                yield return AnimateTransform(move.Rook.transform, rookStart, rookEnd);
                move.Rook.SetCell(move.RookDestination);
                move.Rook.MarkMoved();
            }

            if (promotionChoice != UnitType.None && unit.UnitType == UnitType.Pawn)
            {
                unit.Promote(promotionChoice);
            }

            IsBusy = false;

            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }

        private IEnumerator AnimateTransform(Transform target, Vector3 start, Vector3 end)
        {
            if (moveDuration <= 0f)
            {
                target.position = end;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                target.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            target.position = end;
        }
    }
}
