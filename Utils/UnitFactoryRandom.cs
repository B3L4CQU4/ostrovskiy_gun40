using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;

namespace GamePrototype.Utils {
    public static class UnitFactoryRandom {

        private static readonly Random _random = new();

        public static Player CreatePlayer(
            string name,
            DifficultyConfig config,
            GameDifficulty difficulty
        ) {
            var player = new Player(
                name,
                config.PlayerHealth,
                config.PlayerHealth,
                config.PlayerDamage
            );

            // обязательное оружие
            player.AddItemToInventory(RandomWeapon(difficulty));

            // опциональная броня
            TryAdd(player, RandomArmour(EquipSlot.Armour, difficulty));
            TryAdd(player, RandomArmour(EquipSlot.Helmet, difficulty));
            TryAdd(player, RandomArmour(EquipSlot.Pants, difficulty));

            // расходники
            for (int i = 0; i < config.StartPotions; i++) {
                player.AddItemToInventory(new HealthPotion("Potion"));
            }

            for (int i = 0; i < config.StartGrindstones; i++) {
                player.AddItemToInventory(new Grindstone("Grindstone"));
            }

            return player;
        }

        public static Unit CreateGoblinEnemy(DifficultyConfig config) {
            return new Goblin(
                GameConstants.Goblin,
                config.EnemyHealth,
                config.EnemyHealth,
                config.EnemyDamage
            );
        }

        private static void TryAdd(Player player, EquipItem? item) {
            if (item != null) {
                player.AddItemToInventory(item);
            }
        }

        private static uint Rand(int min, int max) {
            return (uint)_random.Next(min, max + 1);
        }

        // рандомная экипировка 
        private static EquipItem RandomWeapon(GameDifficulty difficulty) {
            return difficulty switch {

                GameDifficulty.Easy => _random.Next(2) == 0
                    ? new Weapon(
                        Rand(9, 12),
                        Rand(18, 25),
                        "Sword"
                      )
                    : new RangeWeapon(
                        Rand(7, 10),
                        Rand(18, 25),
                        "Bow"
                      ),

                GameDifficulty.Hard => _random.Next(2) == 0
                    ? new Weapon(
                        Rand(6, 9),
                        Rand(12, 18),
                        "Rusty Sword"
                      )
                    : new RangeWeapon(
                        Rand(5, 8),
                        Rand(12, 18),
                        "Old Bow"
                      ),

                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static EquipItem? RandomArmour(
            EquipSlot slot,
            GameDifficulty difficulty
        ) {
            var roll = _random.NextDouble();

            return (slot, difficulty) switch {

                // Armour
                (EquipSlot.Armour, GameDifficulty.Easy) when roll < 0.7 =>
                    new Armour(
                        Rand(8, 12),
                        Rand(18, 25),
                        "Armour"
                    ),

                (EquipSlot.Armour, GameDifficulty.Hard) when roll < 0.7 =>
                    new Armour(
                        Rand(5, 8),
                        Rand(12, 18),
                        "Worn Armour"
                    ),

                // Helmet
                (EquipSlot.Helmet, GameDifficulty.Easy) when roll < 0.5 =>
                    new Helmet(
                        Rand(4, 6),
                        Rand(15, 20),
                        "Helmet"
                    ),

                (EquipSlot.Helmet, GameDifficulty.Hard) when roll < 0.5 =>
                    new Helmet(
                        Rand(2, 4),
                        Rand(10, 15),
                        "Cracked Helmet"
                    ),

                // Pants
                (EquipSlot.Pants, GameDifficulty.Easy) when roll < 0.4 =>
                    new Pants(
                        Rand(3, 5),
                        Rand(15, 20),
                        "Pants"
                    ),

                (EquipSlot.Pants, GameDifficulty.Hard) when roll < 0.4 =>
                    new Pants(
                        Rand(1, 3),
                        Rand(10, 15),
                        "Torn Pants"
                    ),

                _ => null
            };
        }
    }
}