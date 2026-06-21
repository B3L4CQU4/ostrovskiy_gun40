using TacticalPrototype.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalPrototype.Units
{
    [DisallowMultipleComponent]
    public sealed class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private CellCoord coordinate;
        [SerializeField] private Transform unitAnchor;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Material baseMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material moveMaterial;
        [SerializeField] private Material attackMaterial;
        [SerializeField] private Material forcedMaterial;
        [SerializeField] private Unit currentUnit;

        private CellHighlight highlight;
        private bool pointerInside;

        public CellCoord Coordinate { get { return coordinate; } }
        public Transform UnitAnchor { get { return unitAnchor != null ? unitAnchor : transform; } }
        public Unit CurrentUnit { get { return currentUnit; } }

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            ApplyMaterial();
        }

        public void Configure(CellCoord coord, Material baseMat, Material hoverMat, Material selectedMat, Material moveMat, Material attackMat, Material forcedMat)
        {
            coordinate = coord;
            baseMaterial = baseMat;
            hoverMaterial = hoverMat;
            selectedMaterial = selectedMat;
            moveMaterial = moveMat;
            attackMaterial = attackMat;
            forcedMaterial = forcedMat;

            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            ApplyMaterial();
        }

        public void SetUnit(Unit unit)
        {
            currentUnit = unit;
        }

        public void ClearUnit(Unit unit)
        {
            if (currentUnit == unit)
            {
                currentUnit = null;
            }
        }

        public void ClearHighlight()
        {
            highlight = CellHighlight.None;
            pointerInside = false;
            ApplyMaterial();
        }

        public void SetHighlight(CellHighlight value)
        {
            highlight = value;
            ApplyMaterial();
        }

        public void SetPointerHover(bool hovered)
        {
            pointerInside = hovered;
            ApplyMaterial();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetPointerHover(true);

            if (currentUnit != null)
            {
                currentUnit.SetHovered(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetPointerHover(false);

            if (currentUnit != null)
            {
                currentUnit.SetHovered(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BattleController.InstanceSelectCell(this);
        }

        private void ApplyMaterial()
        {
            if (targetRenderer == null)
            {
                return;
            }

            Material material = baseMaterial;

            if (highlight == CellHighlight.Selected && selectedMaterial != null)
            {
                material = selectedMaterial;
            }
            else if (highlight == CellHighlight.Attack && attackMaterial != null)
            {
                material = attackMaterial;
            }
            else if (highlight == CellHighlight.Move && moveMaterial != null)
            {
                material = moveMaterial;
            }
            else if (highlight == CellHighlight.Forced && forcedMaterial != null)
            {
                material = forcedMaterial;
            }
            else if (pointerInside && hoverMaterial != null)
            {
                material = hoverMaterial;
            }

            if (material != null)
            {
                targetRenderer.sharedMaterial = material;
            }
        }
    }
}
