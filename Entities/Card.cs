namespace CasinoGame.Entities {
    public struct Card {
        public CardSuit Suit { get; }
        public CardValue Value { get; }

        public Card(CardSuit suit, CardValue value) {
            Suit = suit;
            Value = value;
        }

        public override string ToString() {
            return $"{Value} of {Suit}";
        }
    }
}