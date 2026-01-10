namespace GamePrototype.Utils {
    public sealed class DifficultyConfig {
        public uint PlayerHealth { get; init; }
        public uint PlayerDamage { get; init; }

        public uint EnemyHealth { get; init; }
        public uint EnemyDamage { get; init; }

        public int StartPotions { get; init; }
        public int StartGrindstones { get; init; }
    }
}