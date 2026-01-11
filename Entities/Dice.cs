using System;

namespace CasinoGame.Entities {
    public struct Dice {
        private readonly int Min;
        private readonly int Max;

        private static readonly Random Random = new Random();

        public int Number {
            get {
                return Random.Next(Min, Max + 1);
            }
        }

        public Dice(int min, int max) {
            if (min < 1 || max > int.MaxValue || min > max) {
                throw new WrongDiceNumberException(min > max ? min : max, 1, int.MaxValue);
            }

            Min = min;
            Max = max;
        }
    }
}