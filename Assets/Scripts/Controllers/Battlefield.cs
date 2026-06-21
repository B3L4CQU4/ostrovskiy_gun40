using System.Collections.Generic;
using TacticalPrototype.Controllers.Rules;
using TacticalPrototype.Units;
using UnityEngine;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class Battlefield : MonoBehaviour
    {
        [SerializeField] private Cell[] cells;
        [SerializeField] private Transform checkersUnitsRoot;
        [SerializeField] private Transform chessUnitsRoot;

        private readonly Dictionary<CellCoord, Cell> cellByCoord = new Dictionary<CellCoord, Cell>();
        private GameKind activeGameKind;

        public GameKind ActiveGameKind { get { return activeGameKind; } }

        public IReadOnlyList<Cell> Cells
        {
            get { return cells; }
        }

        public void Configure(Cell[] valueCells, Transform checkersRoot, Transform chessRoot)
        {
            cells = valueCells;
            checkersUnitsRoot = checkersRoot;
            chessUnitsRoot = chessRoot;
        }

        public void Initialize()
        {
            BuildCellMap();
            SetRootActive(checkersUnitsRoot, false);
            SetRootActive(chessUnitsRoot, false);
            ClearHighlights();
        }

        public void ActivateGame(GameKind gameKind)
        {
            activeGameKind = gameKind;
            SetRootActive(checkersUnitsRoot, gameKind == GameKind.Checkers);
            SetRootActive(chessUnitsRoot, gameKind == GameKind.Chess);
            ResetUnits(GetActiveRoot());
            RebuildOccupants();
            ClearHighlights();
        }

        public BoardState CreateState()
        {
            return new BoardState(cells, GetActiveUnits());
        }

        public IEnumerable<Unit> GetActiveUnits()
        {
            Transform root = GetActiveRoot();
            if (root == null)
            {
                yield break;
            }

            Unit[] units = root.GetComponentsInChildren<Unit>(true);
            foreach (Unit unit in units)
            {
                if (unit != null && unit.IsAlive)
                {
                    yield return unit;
                }
            }
        }

        public Cell GetCell(CellCoord coord)
        {
            Cell cell;
            return cellByCoord.TryGetValue(coord, out cell) ? cell : null;
        }

        public void RebuildOccupants()
        {
            foreach (Cell cell in cells)
            {
                if (cell != null && cell.CurrentUnit != null)
                {
                    cell.ClearUnit(cell.CurrentUnit);
                }
            }

            foreach (Unit unit in GetActiveUnits())
            {
                if (unit.CurrentCell != null)
                {
                    unit.CurrentCell.SetUnit(unit);
                }
            }
        }

        public void ClearHighlights()
        {
            foreach (Cell cell in cells)
            {
                if (cell != null)
                {
                    cell.ClearHighlight();
                }
            }
        }

        public void HighlightSelection(Unit selectedUnit, IReadOnlyList<MoveOption> moves)
        {
            ClearHighlights();

            if (selectedUnit != null && selectedUnit.CurrentCell != null)
            {
                selectedUnit.CurrentCell.SetHighlight(CellHighlight.Selected);
            }

            foreach (MoveOption move in moves)
            {
                if (move.Destination == null)
                {
                    continue;
                }

                move.Destination.SetHighlight(move.IsCapture ? CellHighlight.Attack : CellHighlight.Move);
            }
        }

        public void HighlightForcedUnits(IReadOnlyList<MoveOption> moves)
        {
            ClearHighlights();

            foreach (MoveOption move in moves)
            {
                if (move.Unit != null && move.Unit.CurrentCell != null)
                {
                    move.Unit.CurrentCell.SetHighlight(CellHighlight.Forced);
                }
            }
        }

        private void BuildCellMap()
        {
            cellByCoord.Clear();

            foreach (Cell cell in cells)
            {
                if (cell == null)
                {
                    Debug.LogError("Battlefield has an empty cell reference.");
                    continue;
                }

                if (cellByCoord.ContainsKey(cell.Coordinate))
                {
                    Debug.LogError("Duplicate cell coordinate: " + cell.Coordinate);
                    continue;
                }

                cellByCoord.Add(cell.Coordinate, cell);
            }

            if (cellByCoord.Count != BoardState.Size * BoardState.Size)
            {
                Debug.LogError("Battlefield expected 64 cells, found " + cellByCoord.Count + ".");
            }
        }

        private Transform GetActiveRoot()
        {
            if (activeGameKind == GameKind.Checkers)
            {
                return checkersUnitsRoot;
            }

            if (activeGameKind == GameKind.Chess)
            {
                return chessUnitsRoot;
            }

            return null;
        }

        private static void SetRootActive(Transform root, bool active)
        {
            if (root != null)
            {
                root.gameObject.SetActive(active);
            }
        }

        private static void ResetUnits(Transform root)
        {
            if (root == null)
            {
                return;
            }

            Unit[] units = root.GetComponentsInChildren<Unit>(true);
            foreach (Unit unit in units)
            {
                unit.ResetForGame();
            }
        }
    }
}
