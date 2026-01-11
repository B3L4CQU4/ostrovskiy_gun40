using System;

namespace CasinoGame.Entities {
    public class WrongDiceNumberException : Exception {
        public WrongDiceNumberException(int value, int min, int max)
            : base($"Wrong dice number: {value}. Allowed range is {min} to {max}.") {
        }
    }
}