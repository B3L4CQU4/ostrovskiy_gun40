using GamePrototype.Dungeon;
using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;

namespace GamePrototype.Utils {
    public static class DungeonBuilderRandom {

        public static DungeonRoom BuildDungeon(
            DungeonConfig dungeonConfig,
            DifficultyConfig unitConfig
        ) {
            var random = new Random();

            while (true) {
                var start = new DungeonRoom("Enter");
                var final = new DungeonRoom("Final");

                // главный путь (гарантия проходимости)
                var mainPath = GenerateMainPath(
                    start,
                    final,
                    random.Next(dungeonConfig.MinDepth, dungeonConfig.MaxDepth + 1),
                    random
                );

                // боковые ветки (варианты выбора)
                AddBranches(mainPath, random);

                // наполнение
                PopulateRooms(start, mainPath, dungeonConfig, unitConfig, random);

                // проверки графа
                if (IsReachable(start, final) && !HasCycles(start)) {
                    return start;
                }
            }
        }

        // генерация гарантированного пути

        private static List<DungeonRoom> GenerateMainPath(
            DungeonRoom start,
            DungeonRoom final,
            int depth,
            Random random
        ) {
            var path = new List<DungeonRoom> { start };
            var current = start;

            for (int i = 0; i < depth - 1; i++) {
                var next = new DungeonRoom($"Room_{i}");
                current.TrySetDirection(RandomFreeDirection(current, random), next);
                current = next;
                path.Add(next);
            }

            current.TrySetDirection(RandomFreeDirection(current, random), final);
            path.Add(final);

            return path;
        }

        // боковые ветви

        private static void AddBranches(List<DungeonRoom> path, Random random) {
            foreach (var room in path.Skip(1).Take(path.Count - 2)) {
                if (random.NextDouble() < 0.6) {
                    var branch = new DungeonRoom("SideRoom");
                    room.TrySetDirection(RandomFreeDirection(room, random), branch);
                }
            }
        }

        // наполнение комнат

        private static void PopulateRooms(
            DungeonRoom start,
            List<DungeonRoom> mainPath,
            DungeonConfig config,
            DifficultyConfig unitConfig,
            Random random
        ) {
            var candidates = mainPath
                .Skip(1)
                .Take(mainPath.Count - 2)
                .OrderBy(_ => random.Next())
                .ToList();

            Assign(start, candidates, config.EnemyRooms,
                () => new DungeonRoom(
                    "Enemy",
                    UnitFactoryRandom.CreateGoblinEnemy(unitConfig)
                ));

            Assign(start, candidates, config.GoldRooms,
                () => new DungeonRoom("Gold", new Gold()));

            Assign(start, candidates, config.GrindstoneRooms,
                () => new DungeonRoom("Grindstone", new Grindstone("Grindstone")));

            Assign(start, candidates, config.EquipmentRooms,
                () => new DungeonRoom("Equipment", RandomEquipment(random)));
        }

        private static void Assign(
            DungeonRoom start,
            List<DungeonRoom> candidates,
            int count,
            Func<DungeonRoom> factory
        ) {
            for (int i = 0; i < count && candidates.Count > 0; i++) {
                var index = Random.Shared.Next(candidates.Count);
                var oldRoom = candidates[index];
                var newRoom = factory();

                // копируем выходы
                foreach (var kv in oldRoom.Rooms) {
                    newRoom.TrySetDirection(kv.Key, kv.Value);
                }

                // заменяем ссылки в графе
                ReplaceRoomInGraph(start, oldRoom, newRoom);

                candidates.RemoveAt(index);
            }
        }

        // замены комнат

        private static void ReplaceRoomInGraph(
            DungeonRoom start,
            DungeonRoom oldRoom,
            DungeonRoom newRoom
        ) {
            foreach (var room in GetAllRooms(start)) {
                foreach (var kv in room.Rooms.ToList()) {
                    if (kv.Value == oldRoom) {
                        room.Rooms[kv.Key] = newRoom;
                    }
                }
            }
        }

        private static IEnumerable<DungeonRoom> GetAllRooms(DungeonRoom start) {
            var visited = new HashSet<DungeonRoom>();
            var queue = new Queue<DungeonRoom>();
            queue.Enqueue(start);

            while (queue.Count > 0) {
                var room = queue.Dequeue();
                if (!visited.Add(room)) continue;

                yield return room;

                foreach (var next in room.Rooms.Values) {
                    queue.Enqueue(next);
                }
            }
        }

        // проверки

        private static bool IsReachable(DungeonRoom start, DungeonRoom target) {
            var visited = new HashSet<DungeonRoom>();
            var queue = new Queue<DungeonRoom>();
            queue.Enqueue(start);

            while (queue.Count > 0) {
                var room = queue.Dequeue();
                if (!visited.Add(room)) continue;
                if (room == target) return true;

                foreach (var next in room.Rooms.Values) {
                    queue.Enqueue(next);
                }
            }

            return false;
        }

        private static bool HasCycles(DungeonRoom start) {
            var visited = new HashSet<DungeonRoom>();
            var stack = new HashSet<DungeonRoom>();

            return Dfs(start);

            bool Dfs(DungeonRoom room) {
                if (stack.Contains(room)) return true;
                if (!visited.Add(room)) return false;

                stack.Add(room);
                foreach (var next in room.Rooms.Values) {
                    if (Dfs(next)) return true;
                }
                stack.Remove(room);
                return false;
            }
        }

        private static Direction RandomFreeDirection(DungeonRoom room, Random random) {
            var available = Enum
                .GetValues<Direction>()
                .Where(d => !room.Rooms.ContainsKey(d))
                .ToList();

            return available[random.Next(available.Count)];
        }

        private static EquipItem RandomEquipment(Random random) {
            uint Rand(int min, int max) => (uint)random.Next(min, max + 1);

            return random.Next(4) switch {

                // Weapon
                0 => new Weapon(
                    Rand(8, 12),
                    Rand(18, 25),
                    "Sword"
                ),

                // RangeWeapon
                1 => new RangeWeapon(
                    Rand(7, 11),
                    Rand(18, 25),
                    "Bow"
                ),

                // Armour
                2 => new Armour(
                    Rand(8, 12),
                    Rand(20, 30),
                    "Armour"
                ),

                // Helmet
                3 => new Helmet(
                    Rand(4, 6),
                    Rand(15, 20),
                    "Helmet"
                ),

                _ => throw new InvalidOperationException()
            };
        }
    }
}