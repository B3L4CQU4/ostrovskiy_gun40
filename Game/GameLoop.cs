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

        public void StartGame() {
            Initialize();
            Console.WriteLine("Entering the dungeon");
            StartGameLoop();
        }

        #region Game Loop

        private void Initialize() {
            Console.WriteLine("Welcome, player!");
            _dungeon = DungeonBuilder.BuildDungeon();
            Console.WriteLine("Enter your name");
            _player = UnitFactoryDemo.CreatePlayer(Console.ReadLine());
            Console.WriteLine($"Hello {_player.Name}!\n -h for help");
            _inputHandler = new InputHandler(_player);
        }

        private void StartGameLoop() {
            var currentRoom = _dungeon;

            while (currentRoom.IsFinal == false) {
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
            Console.WriteLine("Result: ");
            Console.WriteLine(_player.ToString());
        }

        private void StartRoomEncounter(DungeonRoom currentRoom, out bool success) {
            success = true;
            if (currentRoom.Loot != null) {
                _player.AddItemToInventory(currentRoom.Loot);
            }
            if (currentRoom.Enemy != null) {
                if (_combatManager.StartCombat(_player, currentRoom.Enemy) == _player) {
                    _player.HandleCombatComplete();
                    LootEnemy(currentRoom.Enemy);
                } else {
                    success = false;
                }
            }

            void LootEnemy(Unit enemy) {
                _player.AddItemsFromUnitToInventory(enemy);
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
    }
}
