using System;
using System.Collections.Generic;
using CasinoGame.Entities;

namespace CasinoGame.Games {
    public class BlackjackGame : CasinoGameBase {
        private readonly int _cardsCount;

        private Queue<Card> _deck = null!;
        private List<Card> _playerCards = null!;
        private List<Card> _computerCards = null!;

        public BlackjackGame(int cardsCount) {
            if (cardsCount <= 0) {
                throw new ArgumentException("Cards count must be greater than zero.", nameof(cardsCount));
            }

            _cardsCount = cardsCount;
        }

        protected override void FactoryMethod() {
            _deck = new Queue<Card>();
            _playerCards = new List<Card>();
            _computerCards = new List<Card>();
        }

        private void Initialize() {
            List<Card> cards = new List<Card>();

            foreach (CardSuit suit in Enum.GetValues<CardSuit>()) {
                foreach (CardValue value in Enum.GetValues<CardValue>()) {
                    cards.Add(new Card(suit, value));
                }
            }

            Shuffle(cards);

            for (int i = 0; i < _cardsCount && cards.Count > 0; i++) {
                _deck.Enqueue(cards[0]);
                cards.RemoveAt(0);
            }
        }

        public override void PlayGame() {
            Initialize();
            _playerCards.Clear();
            _computerCards.Clear();

            DealInitialCards();

            while (true) {
                int playerScore = CalculateScore(_playerCards);
                int computerScore = CalculateScore(_computerCards);

                PrintCards(playerScore, computerScore);

                if (playerScore == computerScore && playerScore < 21) {
                    TakeCard(_playerCards);
                    TakeCard(_computerCards);
                    continue;
                }

                if (IsPlayerWinner(playerScore, computerScore)) {
                    OnWinInvoke();
                } else if (IsComputerWinner(playerScore, computerScore)) {
                    OnLooseInvoke();
                } else {
                    OnDrawInvoke();
                }

                break;
            }
        }

        private void DealInitialCards() {
            TakeCard(_playerCards);
            TakeCard(_playerCards);

            TakeCard(_computerCards);
            TakeCard(_computerCards);
        }

        private void TakeCard(List<Card> hand) {
            if (_deck.Count > 0) {
                hand.Add(_deck.Dequeue());
            }
        }

        private static int CalculateScore(List<Card> cards) {
            int score = 0;

            foreach (Card card in cards) {
                score += card.Value switch {
                    CardValue.Jack => 10,
                    CardValue.Queen => 10,
                    CardValue.King => 10,
                    CardValue.Ace => 11,
                    _ => (int)card.Value
                };
            }

            return score;
        }

        private static bool IsPlayerWinner(int player, int computer) {
            return player <= 21 && (computer > 21 || player > computer);
        }

        private static bool IsComputerWinner(int player, int computer) {
            return computer <= 21 && (player > 21 || computer > player);
        }

        private static void Shuffle(List<Card> cards) {
            Random random = new Random();

            for (int i = cards.Count - 1; i > 0; i--) {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        private void PrintCards(int playerScore, int computerScore) {
            Console.WriteLine("=====================================");
            Console.WriteLine("Player cards:");
            foreach (Card card in _playerCards) {
                Console.WriteLine(card);
            }
            Console.WriteLine("=====================================");
            Console.WriteLine($"Player score: {playerScore}");
            Console.WriteLine("=====================================");
            Console.WriteLine("Computer cards:");
            foreach (Card card in _computerCards) {
                Console.WriteLine(card);
            }
            Console.WriteLine("=====================================");
            Console.WriteLine($"Computer score: {computerScore}");
            Console.WriteLine("=====================================");
        }
    }
}