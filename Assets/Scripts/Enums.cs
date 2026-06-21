using System;

namespace TacticalPrototype
{
    public enum GameKind
    {
        None = 0,
        Checkers = 1,
        Chess = 2
    }

    public enum Team
    {
        None = 0,
        White = 1,
        Black = 2
    }

    public enum UnitType
    {
        None = 0,
        Checker = 1,
        CheckerKing = 2,
        Pawn = 10,
        Bishop = 11,
        Rook = 12,
        Queen = 13,
        Knight = 14,
        King = 15
    }

    public enum MoveKind
    {
        Quiet = 0,
        Capture = 1,
        Promotion = 2,
        CastleKingSide = 3,
        CastleQueenSide = 4,
        EnPassant = 5
    }

    public enum TurnPhase
    {
        WaitingForMode = 0,
        WaitingForSelection = 1,
        WaitingForDestination = 2,
        Animating = 3,
        PromotionChoice = 4
    }

    public enum CellHighlight
    {
        None = 0,
        Hover = 1,
        Selected = 2,
        Move = 3,
        Attack = 4,
        Forced = 5
    }

    [Serializable]
    public struct CellCoord : IEquatable<CellCoord>
    {
        public int X;
        public int Y;

        public CellCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(CellCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is CellCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }

        public static CellCoord operator +(CellCoord left, CellCoord right)
        {
            return new CellCoord(left.X + right.X, left.Y + right.Y);
        }

        public static bool operator ==(CellCoord left, CellCoord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CellCoord left, CellCoord right)
        {
            return !left.Equals(right);
        }
    }
}
