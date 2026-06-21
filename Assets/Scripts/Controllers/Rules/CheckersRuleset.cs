using System.Collections.Generic;
using TacticalPrototype.Units;

namespace TacticalPrototype.Controllers.Rules
{
    public sealed class CheckersRuleset : IRuleset
    {
        private static readonly CellCoord[] DiagonalDirections =
        {
            new CellCoord(1, 1),
            new CellCoord(-1, 1),
            new CellCoord(1, -1),
            new CellCoord(-1, -1)
        };

        public GameKind GameKind
        {
            get { return GameKind.Checkers; }
        }

        public IReadOnlyList<MoveOption> GetLegalMoves(BoardState state, Team team, Unit forcedUnit)
        {
            List<MoveOption> allMoves = new List<MoveOption>();
            List<MoveOption> captureMoves = new List<MoveOption>();

            if (forcedUnit != null)
            {
                AddMovesForUnit(state, forcedUnit, team, true, captureMoves, captureMoves);
                return captureMoves;
            }

            foreach (Unit unit in state.UnitsForTeam(team))
            {
                AddMovesForUnit(state, unit, team, false, allMoves, captureMoves);
            }

            return captureMoves.Count > 0 ? captureMoves : allMoves;
        }

        public IReadOnlyList<MoveOption> GetLegalMovesForUnit(BoardState state, Unit unit, Team team, Unit forcedUnit)
        {
            List<MoveOption> allMoves = new List<MoveOption>();
            List<MoveOption> captureMoves = new List<MoveOption>();

            if (unit == null || unit.Team != team || !unit.IsAlive)
            {
                return allMoves;
            }

            AddMovesForUnit(state, unit, team, forcedUnit != null, allMoves, captureMoves);
            return captureMoves.Count > 0 ? captureMoves : allMoves;
        }

        public void OnMoveApplied(BoardState state, MoveOption move, UnitType promotionChoice)
        {
            if (move == null || move.Unit == null || move.Unit.CurrentCell == null)
            {
                return;
            }

            bool reachedWhitePromotion = move.Unit.Team == Team.White && move.Unit.CurrentCell.Coordinate.Y == BoardState.Size - 1;
            bool reachedBlackPromotion = move.Unit.Team == Team.Black && move.Unit.CurrentCell.Coordinate.Y == 0;

            if (move.Unit.UnitType == UnitType.Checker && (reachedWhitePromotion || reachedBlackPromotion))
            {
                move.Unit.Promote(UnitType.CheckerKing);
            }
        }

        private static void AddMovesForUnit(BoardState state, Unit unit, Team team, bool capturesOnly, List<MoveOption> allMoves, List<MoveOption> captureMoves)
        {
            if (unit.UnitType == UnitType.CheckerKing)
            {
                AddKingMoves(state, unit, team, capturesOnly, allMoves, captureMoves);
            }
            else
            {
                AddCheckerMoves(state, unit, team, capturesOnly, allMoves, captureMoves);
            }
        }

        private static void AddCheckerMoves(BoardState state, Unit unit, Team team, bool capturesOnly, List<MoveOption> allMoves, List<MoveOption> captureMoves)
        {
            CellCoord source = unit.CurrentCell.Coordinate;
            int forward = BoardState.Forward(team);

            foreach (CellCoord direction in DiagonalDirections)
            {
                CellCoord adjacent = source + direction;
                CellCoord landing = adjacent + direction;

                if (!state.IsInside(adjacent) || !state.IsInside(landing))
                {
                    continue;
                }

                Unit target = state.GetUnit(adjacent);
                if (target != null && target.Team != team && target.Team != Team.None && state.IsEmpty(landing))
                {
                    Cell destination = state.GetCell(landing);
                    MoveOption move = new MoveOption(unit, unit.CurrentCell, destination, MoveKind.Capture);
                    move.Captures.Add(target);
                    captureMoves.Add(move);
                }
            }

            if (capturesOnly)
            {
                return;
            }

            CellCoord[] forwardDirections =
            {
                new CellCoord(1, forward),
                new CellCoord(-1, forward)
            };

            foreach (CellCoord direction in forwardDirections)
            {
                CellCoord destinationCoord = source + direction;
                if (state.IsEmpty(destinationCoord))
                {
                    Cell destination = state.GetCell(destinationCoord);
                    allMoves.Add(new MoveOption(unit, unit.CurrentCell, destination, MoveKind.Quiet));
                }
            }
        }

        private static void AddKingMoves(BoardState state, Unit unit, Team team, bool capturesOnly, List<MoveOption> allMoves, List<MoveOption> captureMoves)
        {
            CellCoord source = unit.CurrentCell.Coordinate;

            foreach (CellCoord direction in DiagonalDirections)
            {
                bool foundEnemy = false;
                Unit enemy = null;
                CellCoord current = source + direction;

                while (state.IsInside(current))
                {
                    Unit occupant = state.GetUnit(current);

                    if (occupant == null)
                    {
                        Cell destination = state.GetCell(current);

                        if (foundEnemy)
                        {
                            MoveOption capture = new MoveOption(unit, unit.CurrentCell, destination, MoveKind.Capture);
                            capture.Captures.Add(enemy);
                            captureMoves.Add(capture);
                            break;
                        }

                        if (!capturesOnly)
                        {
                            allMoves.Add(new MoveOption(unit, unit.CurrentCell, destination, MoveKind.Quiet));
                        }
                    }
                    else if (occupant.Team == team)
                    {
                        break;
                    }
                    else
                    {
                        if (foundEnemy)
                        {
                            break;
                        }

                        foundEnemy = true;
                        enemy = occupant;
                    }

                    current += direction;
                }
            }
        }
    }
}
