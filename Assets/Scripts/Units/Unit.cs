using TacticalPrototype.Controllers;
using TacticalPrototype.Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TacticalPrototype.Units
{
    [DisallowMultipleComponent]
    public sealed class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Team team;
        [SerializeField] private UnitType unitType;
        [SerializeField] private UnitType startingUnitType;
        [SerializeField] private Cell startingCell;
        [SerializeField] private Cell currentCell;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private GameObject currentVisual;
        [SerializeField] private UnitType currentVisualType;
        [SerializeField] private UnitVisualCatalog visualCatalog;
        [SerializeField] private Renderer[] renderers;

        private bool isAlive = true;
        private bool hasMoved;
        private Cell hoveredCell;

        public Team Team { get { return team; } }
        public UnitType UnitType { get { return unitType; } }
        public Cell StartingCell { get { return startingCell; } }
        public Cell CurrentCell { get { return currentCell; } }
        public bool IsAlive { get { return isAlive && gameObject.activeInHierarchy; } }
        public bool HasMoved { get { return hasMoved; } }

        private void Awake()
        {
            CacheRenderers();
            ApplyVisual();
        }

        public void Configure(Team valueTeam, UnitType valueType, Cell valueStartingCell)
        {
            team = valueTeam;
            unitType = valueType;
            startingUnitType = valueType;
            startingCell = valueStartingCell;
            currentCell = valueStartingCell;
            CacheRenderers();
            ApplyVisual();
        }

        public void ConfigureVisuals(Transform valueVisualRoot, GameObject valueCurrentVisual, UnitVisualCatalog valueVisualCatalog, UnitType valueVisualType)
        {
            visualRoot = valueVisualRoot;
            currentVisual = valueCurrentVisual;
            visualCatalog = valueVisualCatalog;
            currentVisualType = valueVisualType;
            CacheRenderers();
        }

        public void ResetForGame()
        {
            isAlive = true;
            hasMoved = false;
            if (startingUnitType != UnitType.None)
            {
                unitType = startingUnitType;
            }

            gameObject.SetActive(true);
            SetCell(startingCell);
            ApplyVisual();
        }

        public void SetCell(Cell cell)
        {
            ClearHoverState();

            if (currentCell != null)
            {
                currentCell.ClearUnit(this);
            }

            currentCell = cell;

            if (currentCell != null)
            {
                currentCell.SetUnit(this);
                transform.position = currentCell.UnitAnchor.position;
            }
        }

        public void MarkMoved()
        {
            hasMoved = true;
        }

        public void Capture()
        {
            isAlive = false;
            ClearHoverState();

            if (currentCell != null)
            {
                currentCell.ClearUnit(this);
            }

            currentCell = null;
            gameObject.SetActive(false);
        }

        public void Promote(UnitType promotedType)
        {
            unitType = promotedType;
            ApplyVisual();
        }

        public void SetHovered(bool hovered)
        {
            float scale = hovered ? 1.12f : 1f;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHovered(true);

            if (currentCell != null)
            {
                hoveredCell = currentCell;
                currentCell.SetPointerHover(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ClearHoverState();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentCell != null)
            {
                BattleController.InstanceSelectCell(currentCell);
            }
        }

        private void CacheRenderers()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        private void ClearHoverState()
        {
            SetHovered(false);

            if (hoveredCell != null)
            {
                hoveredCell.SetPointerHover(false);
                hoveredCell = null;
            }
        }

        private void ApplyVisual()
        {
            EnsureVisualRoot();

            if (visualCatalog == null)
            {
                Debug.LogError("Unit visual catalog is not assigned for " + name + ".", this);
                return;
            }

            GameObject visualPrefab = visualCatalog.GetVisualPrefab(team, unitType);
            if (visualPrefab == null)
            {
                Debug.LogError("No visual prefab for " + team + " " + unitType + ".", this);
                return;
            }

            if (currentVisual != null && currentVisualType == unitType)
            {
                currentVisual.SetActive(true);
                CacheRenderers();
                return;
            }

            ReplaceCurrentVisual(visualPrefab);
        }

        private void EnsureVisualRoot()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }
        }

        private void ReplaceCurrentVisual(GameObject visualPrefab)
        {
            EnsureVisualRoot();

            if (currentVisual != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(currentVisual);
                }
                else
                {
                    DestroyImmediate(currentVisual);
                }
            }

            currentVisual = Instantiate(visualPrefab, visualRoot);
            currentVisual.name = visualPrefab.name;
            currentVisual.transform.localPosition = Vector3.zero;
            currentVisual.transform.localRotation = Quaternion.identity;
            currentVisual.transform.localScale = Vector3.one;
            currentVisualType = unitType;
            CacheRenderers();
        }
    }
}
