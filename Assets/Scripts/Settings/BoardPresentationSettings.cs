using UnityEngine;

namespace TacticalPrototype.Settings
{
    public sealed class BoardPresentationSettings : ScriptableObject
    {
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float unitHeight = 0.35f;

        public float CellSize { get { return cellSize; } }
        public float UnitHeight { get { return unitHeight; } }
    }
}
