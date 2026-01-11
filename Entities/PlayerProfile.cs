namespace CasinoGame.Entities {
    public class PlayerProfile {
        public const int MaxBank = 1_000_000;

        public string Name { get; set; }

        public int Bank { get; set; }

        public PlayerProfile(string name, int bank) {
            Name = name;
            Bank = bank;
        }

        public PlayerProfile() {
            Name = string.Empty;
            Bank = 0;
        }
    }
}