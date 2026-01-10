namespace GamePrototype.Utils {
    public sealed class DungeonConfig {
        public int EnemyRooms { get; init; }
        public int GoldRooms { get; init; }
        public int GrindstoneRooms { get; init; }
        public int EquipmentRooms { get; init; }
        public int MinDepth { get; init; } = 5;
        public int MaxDepth { get; init; } = 8;
    }
}