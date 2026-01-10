using GamePrototype.Combat;
using GamePrototype.Dungeon;
using GamePrototype.Units;
using GamePrototype.Utils;

namespace GamePrototype.Game {
    public sealed class GameLoop {

        private Player _player;
        private DungeonRoom _dungeon;
        private readonly CombatManager _combatManager = new CombatManager();
        private InputHandler _inputHandler;
        private DifficultyConfig _unitConfig;
        private GameDifficulty _difficulty;

        public void StartGame() {
            Initialize();
            Console.WriteLine("Entering the dungeon");
            StartGameLoop();
        }

        #region Initialization

        private void Initialize() {
            Console.WriteLine("Welcome!");

            // выбор сложности
            _difficulty = AskGameDifficulty();

            // конфиги
            var dungeonConfig = CreateDungeonConfig(_difficulty);
            _unitConfig = CreateUnitConfig(_difficulty);

            // генерация данжа
            _dungeon = DungeonBuilderRandom.BuildDungeon(dungeonConfig, _unitConfig);

            // генерация персонажа
            Console.WriteLine("Enter your name:");
            var name = Console.ReadLine();

            while (true) {
                _player = UnitFactoryRandom.CreatePlayer(
                    name,
                    _unitConfig,
                    _difficulty
                );

                Console.WriteLine("Generated character:");
                Console.WriteLine("=================================");
                Console.WriteLine(_player);
                Console.WriteLine("=================================");
                Console.WriteLine("1 - Start game");
                Console.WriteLine("2 - Regenerate character");

                var choice = Console.ReadLine();

                if (choice == "1") break;
                if (choice != "2") Console.WriteLine("Invalid choice.");
            }

            _inputHandler = new InputHandler(_player);
            Console.WriteLine($"Hello {_player.Name}!\n -h for help");
        }

        #endregion

        #region Game Loop

        private void StartGameLoop() {
            var currentRoom = _dungeon;

            while (!currentRoom.IsFinal) {
                StartRoomEncounter(currentRoom, out var success);
                if (!success) {
                    Console.WriteLine("Game over!");
                    return;
                }

                DisplayRouteOptions(currentRoom);
                var direction = _inputHandler.ReadDirection(currentRoom);
                currentRoom = currentRoom.Rooms[direction];
            }

            Console.WriteLine($"Congratulations, {_player.Name}");
            Console.WriteLine("Result:");
            Console.WriteLine(_player);
        }

        private void StartRoomEncounter(DungeonRoom currentRoom, out bool success) {
            success = true;

            if (currentRoom.Loot != null) {
                Console.WriteLine($"loot added: {currentRoom.Loot.Name}");
                _player.AddItemToInventory(currentRoom.Loot);
            }

            if (currentRoom.Enemy != null) {
                if (_combatManager.StartCombat(_player, currentRoom.Enemy) == _player) {
                    _player.HandleCombatComplete();
                    _player.AddItemsFromUnitToInventory(currentRoom.Enemy);
                } else {
                    success = false;
                }
            }
        }

        private void DisplayRouteOptions(DungeonRoom currentRoom) {
            Console.WriteLine("============================================");
            Console.WriteLine("Where to go?");
            foreach (var room in currentRoom.Rooms) {
                Console.WriteLine(room.Key switch {
                    Direction.Left => "A - Left",
                    Direction.Forward => "W - Forward",
                    Direction.Right => "D - Right",
                    _ => ""
                });
            }
            Console.WriteLine("============================================");
        }

        #endregion

        #region Difficulty Selection

        // выбор сложности
        private GameDifficulty AskGameDifficulty() {
            while (true) {
                Console.WriteLine("Select game difficulty:");
                Console.WriteLine("1 - Easy");
                Console.WriteLine("2 - Hard");

                var input = Console.ReadLine();

                if (input == "1") return GameDifficulty.Easy;
                if (input == "2") return GameDifficulty.Hard;

                Console.WriteLine("Invalid choice.");
            }
        }

        // конфиг подземелья
        private DungeonConfig CreateDungeonConfig(GameDifficulty difficulty) {
            return difficulty switch {
                GameDifficulty.Easy => new DungeonConfig {
                    EnemyRooms = 2,
                    GoldRooms = 3,
                    GrindstoneRooms = 2,
                    EquipmentRooms = 2,
                    MinDepth = 4,
                    MaxDepth = 6
                },
                GameDifficulty.Hard => new DungeonConfig {
                    EnemyRooms = 5,
                    GoldRooms = 1,
                    GrindstoneRooms = 1,
                    EquipmentRooms = 1,
                    MinDepth = 7,
                    MaxDepth = 10
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        // конфиг юнитов (игрок + враги)
        private readonly Random _random = new();

        private DifficultyConfig CreateUnitConfig(GameDifficulty difficulty) {
            return difficulty switch {
                GameDifficulty.Easy => new DifficultyConfig {
                    PlayerHealth = RandomRange(36, 44),
                    PlayerDamage = RandomRange(6, 8),
                    EnemyHealth = 60,
                    EnemyDamage = 2,
                    StartPotions = 3,
                    StartGrindstones = 3
                },
                GameDifficulty.Hard => new DifficultyConfig {
                    PlayerHealth = RandomRange(26, 34),
                    PlayerDamage = RandomRange(5, 7),
                    EnemyHealth = 100,
                    EnemyDamage = 4,
                    StartPotions = 1,
                    StartGrindstones = 2
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private uint RandomRange(int min, int max) {
            return (uint)_random.Next(min, max + 1);
        }

        #endregion
    }
}