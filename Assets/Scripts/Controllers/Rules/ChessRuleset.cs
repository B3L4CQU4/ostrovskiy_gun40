using System.Collections.Generic;
using TacticalPrototype.Units;

namespace TacticalPrototype.Controllers.Rules
{
    public sealed class ChessRuleset : IRuleset
    {
        private static readonly CellCoord[] BishopDirections =
        {
            new CellCoord(1, 1),
            new CellCoord(-1, 1),
            new CellCoord(1, -1),
            new CellCoord(-1, -1)
        };

        private static readonly CellCoord[] RookDirections =
        {
            new CellCoord(1, 0),
            new CellCoord(-1, 0),
            new CellCoord(0, 1),
            new CellCoord(0, -1)
        };

        private static readonly CellCoord[] KnightOffsets =
        {
            new CellCoord(1, 2),
            new CellCoord(2, 1),
            new CellCoord(-1, 2),
            new CellCoord(-2, 1),
            new CellCoord(1, -2),
            new CellCoord(2, -1),
            new CellCoord(-1, -2),
            new CellCoord(-2, -1)
        };

        private Cell enPassantTarget;
        private Unit enPassantVictim;

        public GameKind GameKind
        {
            get { return GameKind.Chess; }
        }

        public IReadOnlyList<MoveOption> GetLegalMoves(BoardState state, Team team, Unit forcedUnit)
        {
            List<MoveOption> result = new List<MoveOption>();

            foreach (Unit unit in state.UnitsForTeam(team))
            {
                result.AddRange(GetLegalMovesForUnit(state, unit, team, null));
            }

            return result;
        }

        public IReadOnlyList<MoveOption> GetLegalMovesForUnit(BoardState state, Unit unit, Team team, Unit forcedUnit)
        {
            List<MoveOption> result = new List<MoveOption>();

            if (unit == null || unit.Team != team || !unit.IsAlive)
            {
                return result;
            }

            List<MoveOption> pseudoMoves = new List<MoveOption>();
            AddPseudoMoves(state, unit, pseudoMoves, true);

            foreach (MoveOption move in pseudoMoves)
            {
                if (!WouldLeaveKingInCheck(state, move, team))
                {
                    result.Add(move);
                }
            }

            return result;
        }

        public void OnMoveApplied(BoardState state, MoveOption move, UnitType promotionChoice)
        {
            enPassantTarget = null;
            enPassantVictim = null;

            if (move == null || move.Unit == null)
            {
                return;
            }

            if (move.Unit.UnitType == UnitType.Pawn)
            {
                int distance = move.Destination.Coordinate.Y - move.Source.Coordinate.Y;
                if (distance == 2 || distance == -2)
                {
                    int skippedY = (move.Source.Coordinate.Y + move.Destination.Coordinate.Y) / 2;
                    enPassantTarget = state.GetCell(new CellCoord(move.Source.Coordinate.X, skippedY));
                    enPassantVictim = move.Unit;
                }
            }
        }

        private void AddPseudoMoves(BoardState state, Unit unit, List<MoveOption> moves, bool includeCastling)
        {
            switch (unit.UnitType)
            {
                case UnitType.Pawn:
                    AddPawnMoves(state, unit, moves);
                    break;
                case UnitType.Bishop:
                    AddSlidingMoves(state, unit, moves, BishopDirections);
                    break;
                case UnitType.Rook:
                    AddSlidingMoves(state, unit, moves, RookDirections);
                    break;
                case UnitType.Queen:
                    AddSlidingMoves(state, unit, moves, BishopDirections);
                    AddSlidingMoves(state, unit, moves, RookDirections);
                    break;
                case UnitType.Knight:
                    AddKnightMoves(state, unit, moves);
                    break;
                case UnitType.King:
                    AddKingMoves(state, unit, moves, includeCastling);
                    break;
            }
        }

        private void AddPawnMoves(BoardState state, Unit unit, List<MoveOption> moves)
        {
            CellCoord source = unit.CurrentCell.Coordinate;
            int forward = BoardState.Forward(unit.Team);
            CellCoord oneForward = new CellCoord(source.X, source.Y + forward);

            if (state.IsEmpty(oneForward))
            {
                AddPawnMove(state, unit, oneForward, null, MoveKind.Quiet, moves);

                int startRank = unit.Team == Team.White ? 1 : 6;
                CellCoord twoForward = new CellCoord(source.X, source.Y + forward * 2);
                if (!unit.HasMoved && source.Y == startRank && state.IsEmpty(twoForward))
                {
                    AddPawnMove(state, unit, twoForward, null, MoveKind.Quiet, moves);
                }
            }

            for (int dx = -1; dx <= 1; dx += 2)
            {
                CellCoord target = new CellCoord(source.X + dx, source.Y + forward);
                if (!state.IsInside(target))
                {
                    continue;
                }

                Unit occupant = state.GetUnit(target);
                if (occupant != null && occupant.Team != unit.Team && occupant.Team != Team.None)
                {
                    AddPawnMove(state, unit, target, occupant, MoveKind.Capture, moves);
                }
                else if (enPassantTarget != null && enPassantTarget.Coordinate == target && enPassantVictim != null && enPassantVictim.Team != unit.Team)
                {
                    MoveOption move = new MoveOption(unit, unit.CurrentCell, enPassantTarget, MoveKind.EnPassant);
                    move.Captures.Add(enPassantVictim);
                    moves.Add(move);
                }
            }
        }

        private static void AddPawnMove(BoardState state, Unit unit, CellCoord destinationCoord, Unit capture, MoveKind kind, List<MoveOption> moves)
        {
            Cell destination = state.GetCell(destinationCoord);
            if (destination == null)
            {
                return;
            }

            bool promotes = unit.Team == Team.White && destinationCoord.Y == BoardState.Size - 1 ||
                            unit.Team == Team.Black && destinationCoord.Y == 0;

            MoveOption move = new MoveOption(unit, unit.CurrentCell, destination, promotes ? MoveKind.Promotion : kind);
            if (capture != null)
            {
                move.Captures.Add(capture);
            }

            if (promotes)
            {
                move.RequiresPromotionChoice = true;
                move.PromotionChoices.Add(UnitType.Queen);
                move.PromotionChoices.Add(UnitType.Rook);
                move.PromotionChoices.Add(UnitType.Bishop);
                move.PromotionChoices.Add(UnitType.Knight);
            }

            moves.Add(move);
        }

        private static void AddSlidingMoves(BoardState state, Unit unit, List<MoveOption> moves, CellCoord[] directions)
        {
            CellCoord source = unit.CurrentCell.Coordinate;

            foreach (CellCoord direction in directions)
            {
                CellCoord current = source + direction;
                while (state.IsInside(current))
                {
                    Unit occupant = state.GetUnit(current);
                    if (occupant == null)
                    {
                        moves.Add(new MoveOption(unit, unit.CurrentCell, state.GetCell(current), MoveKind.Quiet));
                    }
                    else
                    {
                        if (occupant.Team != unit.Team && occupant.Team != Team.None)
                        {
                            MoveOption capture = new MoveOption(unit, unit.CurrentCell, state.GetCell(current), MoveKind.Capture);
                            capture.Captures.Add(occupant);
                            moves.Add(capture);
                        }

                        break;
                    }

                    current += direction;
                }
            }
        }

        private static void AddKnightMoves(BoardState state, Unit unit, List<MoveOption> moves)
        {
            CellCoord source = unit.CurrentCell.Coordinate;

            foreach (CellCoord offset in KnightOffsets)
            {
                CellCoord destinationCoord = source + offset;
                if (!state.IsInside(destinationCoord))
                {
                    continue;
                }

                Unit occupant = state.GetUnit(destinationCoord);
                if (occupant == null)
                {
                    moves.Add(new MoveOption(unit, unit.CurrentCell, state.GetCell(destinationCoord), MoveKind.Quiet));
                }
                else if (occupant.Team != unit.Team && occupant.Team != Team.None)
                {
                    MoveOption capture = new MoveOption(unit, unit.CurrentCell, state.GetCell(destinationCoord), MoveKind.Capture);
                    capture.Captures.Add(occupant);
                    moves.Add(capture);
                }
            }
        }

        private void AddKingMoves(BoardState state, Unit unit, List<MoveOption> moves, bool includeCastling)
        {
            CellCoord source = unit.CurrentCell.Coordinate;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    CellCoord destinationCoord = new CellCoord(source.X + dx, source.Y + dy);
                    if (!state.IsInside(destinationCoord))
                    {
                        continue;
                    }

                    Unit occupant = state.GetUnit(destinationCoord);
                    if (occupant == null)
                    {
                        moves.Add(new MoveOption(unit, unit.CurrentCell, state.GetCell(destinationCoord), MoveKind.Quiet));
                    }
                    else if (occupant.Team != unit.Team && occupant.Team != Team.None)
                    {
                        MoveOption capture = new MoveOption(unit, unit.CurrentCell, state.GetCell(destinationCoord), MoveKind.Capture);
                        capture.Captures.Add(occupant);
                        moves.Add(capture);
                    }
                }
            }

            if (includeCastling)
            {
                AddCastlingMoves(state, unit, moves);
            }
        }

        private void AddCastlingMoves(BoardState state, Unit king, List<MoveOption> moves)
        {
            if (king.HasMoved)
            {
                return;
            }

            int homeRank = king.Team == Team.White ? 0 : 7;
            CellCoord kingCoord = king.CurrentCell.Coordinate;
            if (kingCoord.X != 4 || kingCoord.Y != homeRank || IsKingInCheck(state, king.Team, state.CreatePositionMap()))
            {
                return;
            }

            TryAddCastle(state, king, homeRank, 7, new CellCoord(6, homeRank), new CellCoord(5, homeRank), MoveKind.CastleKingSide, moves);
            TryAddCastle(state, king, homeRank, 0, new CellCoord(2, homeRank), new CellCoord(3, homeRank), MoveKind.CastleQueenSide, moves);
        }

        private void TryAddCastle(BoardState state, Unit king, int homeRank, int rookX, CellCoord kingDestination, CellCoord rookDestination, MoveKind kind, List<MoveOption> moves)
        {
            Unit rook = state.GetUnit(new CellCoord(rookX, homeRank));
            if (rook == null || rook.Team != king.Team || rook.UnitType != UnitType.Rook || rook.HasMoved)
            {
                return;
            }

            int direction = rookX > king.CurrentCell.Coordinate.X ? 1 : -1;
            for (int x = king.CurrentCell.Coordinate.X + direction; x != rookX; x += direction)
            {
                if (!state.IsEmpty(new CellCoord(x, homeRank)))
                {
                    return;
                }
            }

            Dictionary<Unit, CellCoord> positions = state.CreatePositionMap();
            Team opponent = BoardState.Opponent(king.Team);
            if (IsSquareAttacked(state, rookDestination, opponent, positions) || IsSquareAttacked(state, kingDestination, opponent, positions))
            {
                return;
            }

            MoveOption move = new MoveOption(king, king.CurrentCell, state.GetCell(kingDestination), kind);
            move.Rook = rook;
            move.RookDestination = state.GetCell(rookDestination);
            moves.Add(move);
        }

        private bool WouldLeaveKingInCheck(BoardState state, MoveOption move, Team team)
        {
            Dictionary<Unit, CellCoord> positions = state.CreatePositionMap();

            foreach (Unit capture in move.Captures)
            {
                positions.Remove(capture);
            }

            positions[move.Unit] = move.Destination.Coordinate;

            if (move.Rook != null && move.RookDestination != null)
            {
                positions[move.Rook] = move.RookDestination.Coordinate;
            }

            return IsKingInCheck(state, team, positions);
        }

        private bool IsKingInCheck(BoardState state, Team team, Dictionary<Unit, CellCoord> positions)
        {
            Unit king = null;
            CellCoord kingCoord = default(CellCoord);

            foreach (KeyValuePair<Unit, CellCoord> pair in positions)
            {
                if (pair.Key.Team == team && pair.Key.UnitType == UnitType.King)
                {
                    king = pair.Key;
                    kingCoord = pair.Value;
                    break;
                }
            }

            if (king == null)
            {
                return false;
            }

            return IsSquareAttacked(state, kingCoord, BoardState.Opponent(team), positions);
        }

        private bool IsSquareAttacked(BoardState state, CellCoord square, Team byTeam, Dictionary<Unit, CellCoord> positions)
        {
            foreach (KeyValuePair<Unit, CellCoord> pair in positions)
            {
                Unit unit = pair.Key;
                if (unit.Team != byTeam)
                {
                    continue;
                }

                CellCoord source = pair.Value;
                if (AttacksSquare(state, unit, source, square, positions))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AttacksSquare(BoardState state, Unit unit, CellCoord source, CellCoord target, Dictionary<Unit, CellCoord> positions)
        {
            int dx = target.X - source.X;
            int dy = target.Y - source.Y;

            switch (unit.UnitType)
            {
                case UnitType.Pawn:
                    return dy == BoardState.Forward(unit.Team) && (dx == 1 || dx == -1);
                case UnitType.Knight:
                    return (Abs(dx) == 1 && Abs(dy) == 2) || (Abs(dx) == 2 && Abs(dy) == 1);
                case UnitType.King:
                    return Abs(dx) <= 1 && Abs(dy) <= 1 && (dx != 0 || dy != 0);
                case UnitType.Bishop:
                    return Abs(dx) == Abs(dy) && IsVirtualPathClear(source, target, positions);
                case UnitType.Rook:
                    return (dx == 0 || dy == 0) && IsVirtualPathClear(source, target, positions);
                case UnitType.Queen:
                    return (Abs(dx) == Abs(dy) || dx == 0 || dy == 0) && IsVirtualPathClear(source, target, positions);
            }

            return false;
        }

        private static bool IsVirtualPathClear(CellCoord source, CellCoord target, Dictionary<Unit, CellCoord> positions)
        {
            int stepX = Sign(target.X - source.X);
            int stepY = Sign(target.Y - source.Y);
            CellCoord current = new CellCoord(source.X + stepX, source.Y + stepY);

            while (current != target)
            {
                if (GetVirtualOccupant(positions, current) != null)
                {
                    return false;
                }

                current = new CellCoord(current.X + stepX, current.Y + stepY);
            }

            return true;
        }

        private static Unit GetVirtualOccupant(Dictionary<Unit, CellCoord> positions, CellCoord coord)
        {
            foreach (KeyValuePair<Unit, CellCoord> pair in positions)
            {
                if (pair.Value == coord)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        private static int Abs(int value)
        {
            return value < 0 ? -value : value;
        }

        private static int Sign(int value)
        {
            if (value > 0)
            {
                return 1;
            }

            return value < 0 ? -1 : 0;
        }
    }
}
