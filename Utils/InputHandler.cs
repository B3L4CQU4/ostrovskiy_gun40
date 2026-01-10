using GamePrototype.Dungeon;
using GamePrototype.Units;

namespace GamePrototype.Utils {
    public sealed class InputHandler {
        private readonly Player _player;

        public InputHandler(Player player) {
            _player = player;
        }

        ///
        /// Читает ввод до тех пор, пока не будет получена команда движения.
        /// - a/w/d возвращает Direction
        /// -h печатает команды
        /// -i печатает инвентарь
        /// -use использование предмета
        public Direction ReadDirection(DungeonRoom currentRoom) {
            while (true) {
                var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(input)) {
                    Console.WriteLine("Empty input. Type -h for help.");
                    continue;
                }

                if (input == "-h") {
                    PrintHelp(currentRoom);
                    continue;
                }

                if (input == "-i") {
                    PrintInventory();
                    continue;
                }

                if (input.StartsWith("-use")) {
                    HandleUseCommand(input);
                    continue;
                }

                if (input.StartsWith("-e")) {
                    HandleEquipCommand(input);
                    continue;
                }

                if (TryMapToDirection(input, out var direction)) {
                    if (!currentRoom.Rooms.ContainsKey(direction)) {
                        Console.WriteLine("You can't go that way!");
                        continue;
                    }

                    return direction;
                }

                Console.WriteLine("Unknown command. Type -h for help.");
            }
        }

        private static bool TryMapToDirection(string input, out Direction direction) {
            switch (input) {
                case "a":
                    direction = Direction.Left;
                    return true;

                case "w":
                    direction = Direction.Forward;
                    return true;

                case "d":
                    direction = Direction.Right;
                    return true;

                default:
                    direction = default;
                    return false;
            }
        }

        private static void PrintHelp(DungeonRoom currentRoom) {
            Console.WriteLine("============================================");
            Console.WriteLine("Commands:");
            Console.WriteLine("  A  - go Left");
            Console.WriteLine("  W  - go Forward");
            Console.WriteLine("  D  - go Right");
            Console.WriteLine("  -i - show inventory");
            Console.WriteLine("  -h - show this help");
            Console.WriteLine("  -use p (or potion) - use potion");
            Console.WriteLine("  -use g (or grindstone) - use grindstone");

            Console.WriteLine();
            Console.WriteLine("Available directions from this room:");
            foreach (var kv in currentRoom.Rooms) {
                Console.WriteLine($"  {kv.Key}");
            }
            Console.WriteLine("============================================");
        }

        private void PrintInventory() {
            Console.WriteLine("============================================");
            Console.WriteLine("Inventory / Player info:");
            Console.WriteLine(_player.ToString());
            Console.WriteLine("============================================");
        }
        private void HandleUseCommand(string input) {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2) {
                Console.WriteLine("Usage: -use p (or potion) | -use g (or grindstone)");
                return;
            }

            var arg = parts[1].ToLowerInvariant();

            if (arg is "p" or "potion") {
                if (_player.TryUseHealthPotion()) {
                    Console.WriteLine("You used a potion (+7 HP).");
                    Console.WriteLine($"Health: {_player.Health}/{_player.MaxHealth}");
                } else {
                    Console.WriteLine("You don't have a potion.");
                }
                return;
            }

            if (arg is "g" or "grindstone") {
                if (!_player.TryUseGrindstone()) {
                    Console.WriteLine("You don't have a grindstone.");
                }
                return;
            }

            Console.WriteLine($"Unknown item '{arg}'. Try: -use potion | -use grindstone");
        }

        private void HandleEquipCommand(string input) {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !int.TryParse(parts[1], out var index)) {
                Console.WriteLine("Usage: -e {inventory index}");
                return;
            }

            _player.TryEquipFromInventory(index);
        }
    }
}