using System.Collections.Generic;
using TacticalPrototype.Units;
using UnityEngine;

namespace TacticalPrototype.Controllers.Rules
{
    public sealed class MoveOption
    {
        public readonly Unit Unit;
        public readonly Cell Source;
        public readonly Cell Destination;
        public readonly MoveKind Kind;
        public readonly List<Unit> Captures = new List<Unit>();
        public readonly List<UnitType> PromotionChoices = new List<UnitType>();

        public Unit Rook;
        public Cell RookDestination;
        public bool RequiresPromotionChoice;

        public MoveOption(Unit unit, Cell source, Cell destination, MoveKind kind)
        {
            Unit = unit;
            Source = source;
            Destination = destination;
            Kind = kind;
        }

        public bool IsCapture
        {
            get { return Captures.Count > 0 || Kind == MoveKind.EnPassant; }
        }

        public bool MatchesDestination(Cell cell)
        {
            return Destination == cell;
        }
    }

    public interface IRuleset
    {
        GameKind GameKind { get; }
        IReadOnlyList<MoveOption> GetLegalMoves(BoardState state, Team team, Unit forcedUnit);
        IReadOnlyList<MoveOption> GetLegalMovesForUnit(BoardState state, Unit unit, Team team, Unit forcedUnit);
        void OnMoveApplied(BoardState state, MoveOption move, UnitType promotionChoice);
    }

    public sealed class BoardState
    {
        public const int Size = 8;

        private readonly Cell[,] cells = new Cell[Size, Size];
        private readonly List<Unit> units = new List<Unit>();

        public BoardState(IEnumerable<Cell> sourceCells, IEnumerable<Unit> sourceUnits)
        {
            foreach (Cell cell in sourceCells)
            {
                if (cell == null)
                {
                    continue;
                }

                CellCoord coord = cell.Coordinate;
                if (IsInside(coord))
                {
                    cells[coord.X, coord.Y] = cell;
                }
            }

            foreach (Unit unit in sourceUnits)
            {
                if (unit != null && unit.IsAlive && unit.CurrentCell != null)
                {
                    units.Add(unit);
                }
            }
        }

        public IReadOnlyList<Unit> Units
        {
            get { return units; }
        }

        public bool IsInside(CellCoord coord)
        {
            return coord.X >= 0 && coord.X < Size && coord.Y >= 0 && coord.Y < Size;
        }

        public Cell GetCell(CellCoord coord)
        {
            if (!IsInside(coord))
            {
                return null;
            }

            return cells[coord.X, coord.Y];
        }

        public Unit GetUnit(CellCoord coord)
        {
            Cell cell = GetCell(coord);
            return cell != null ? cell.CurrentUnit : null;
        }

        public bool IsEmpty(CellCoord coord)
        {
            return GetCell(coord) != null && GetUnit(coord) == null;
        }

        public bool IsAlly(CellCoord coord, Team team)
        {
            Unit unit = GetUnit(coord);
            return unit != null && unit.Team == team;
        }

        public bool IsEnemy(CellCoord coord, Team team)
        {
            Unit unit = GetUnit(coord);
            return unit != null && unit.Team != team && unit.Team != Team.None;
        }

        public IEnumerable<Unit> UnitsForTeam(Team team)
        {
            foreach (Unit unit in units)
            {
                if (unit.Team == team && unit.IsAlive)
                {
                    yield return unit;
                }
            }
        }

        public Unit FindKing(Team team)
        {
            foreach (Unit unit in units)
            {
                if (unit.Team == team && unit.UnitType == UnitType.King && unit.IsAlive)
                {
                    return unit;
                }
            }

            return null;
        }

        public Dictionary<Unit, CellCoord> CreatePositionMap()
        {
            Dictionary<Unit, CellCoord> result = new Dictionary<Unit, CellCoord>();

            foreach (Unit unit in units)
            {
                if (unit.CurrentCell != null && unit.IsAlive)
                {
                    result[unit] = unit.CurrentCell.Coordinate;
                }
            }

            return result;
        }

        public static Team Opponent(Team team)
        {
            return team == Team.White ? Team.Black : Team.White;
        }

        public static int Forward(Team team)
        {
            return team == Team.White ? 1 : -1;
        }

        public static string CoordToName(CellCoord coord)
        {
            char file = (char)('A' + coord.X);
            return string.Format("{0}{1}", file, coord.Y + 1);
        }

        public static Vector3 CoordToWorld(CellCoord coord, float cellSize)
        {
            return new Vector3((coord.X - 3.5f) * cellSize, 0f, (coord.Y - 3.5f) * cellSize);
        }
    }
}
