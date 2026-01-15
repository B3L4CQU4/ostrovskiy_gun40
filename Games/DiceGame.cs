using System;
using System.Collections.Generic;
using CasinoGame.Entities;

namespace CasinoGame.Games {
    public class DiceGame : CasinoGameBase {
        private int _diceCount;
        private int _minValue;
        private int _maxValue;

        private List<Dice> _dices = new();

        public DiceGame(int diceCount, int minValue, int maxValue) {
            if (diceCount <= 0) {
                throw new ArgumentException("Dice count must be greater than zero.", nameof(diceCount));
            }

            _diceCount = diceCount;
            _minValue = minValue;
            _maxValue = maxValue;

            Initialize();
        }

        protected override void FactoryMethod() {
            _dices = new List<Dice>();
        }

        private void Initialize() {
            _dices.Clear();

            for (int i = 0; i < _diceCount; i++) {
                _dices.Add(new Dice(_minValue, _maxValue));
            }
        }

        public override void PlayGame() {
            Console.WriteLine("=====================================");
            int playerScore = RollDices("Player");
            int computerScore = RollDices("Computer");
            Console.WriteLine($"Player score: {playerScore}");
            Console.WriteLine($"Computer score: {computerScore}");
            Console.WriteLine("=====================================");
            if (playerScore > computerScore)
                OnWinInvoke();
            else if (playerScore < computerScore)
                OnLooseInvoke();
            else
                OnDrawInvoke();
        }

        private int RollDices(string owner) {
            int sum = 0;
            Console.Write($"{owner} rolls: ");

            foreach (Dice dice in _dices) {
                int value = dice.Number;
                sum += value;
                Console.Write($"{value} ");
            }

            Console.WriteLine();
            return sum;
        }
    }
}
