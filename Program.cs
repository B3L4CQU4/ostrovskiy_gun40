using System;

namespace MyApp {
    class Program {
        public class Unit {
            // Здоровье
            private float _health;

            // Имя
            public string Name { get; }
            // Здоровье public
            public float Health => _health;

            // Урон
            public Interval Damage { get; }

            // Броня
            public float Armor { get; }

            // Конструкторы
            public Unit() : this("Unknown Unit") { }

            // Конструктор с именем (урон по умолчанию)
            public Unit(string name) : this(name, 0, 10) { }

            // Конструктор с именем и диапазоном урона
            public Unit(string name, int minDamage, int maxDamage) {
                Name = name;
                Damage = new Interval(Math.Max(0, minDamage), Math.Max(0, maxDamage));
                Armor = 0.6f;
                _health = 100f;
            }

            // Получение фактического здоровья
            public float GetRealHealth() {
                return _health * (1f + Armor);
            }

            // Получение урона
            public bool SetDamage(float value) {
                _health -= value * Armor;
                return _health <= 0f;
            }

            // Получение случайного урона от юнита
            public int GetDamage() {
                return Damage.Get();
            }

            public override string ToString() {
                return $"Юнит: {Name}, Здоровье: {Health:F1}, Урон: {Damage}, Броня: {Armor:F1}";
            }
        }

        public class Weapon {
            // Прочность
            private float _durability;

            // Имя
            public string Name { get; }

            // Урон оружия
            public Interval Damage { get; private set; }

            // Конструкторы
            public Weapon(string name) {
                Name = name;
                _durability = 1f;
                Damage = new Interval(1, 10);
            }

            public Weapon(string name, int minDamage, int maxDamage) : this(name) {
                SetDamageParams(minDamage, maxDamage);
            }

            // Установка значений урона
            public void SetDamageParams(int minDamage, int maxDamage) {
                int originalMin = minDamage;
                int originalMax = maxDamage;

                // Корректируем отдельные значения
                if (minDamage < 1) {
                    minDamage = 1;
                    Console.WriteLine($"Для оружия '{Name}' выполнена форсированная установка минимального значения урона: 1.");
                }

                if (maxDamage <= 1) {
                    maxDamage = 10;
                    Console.WriteLine($"Для оружия '{Name}' установлено максимальное значение урона: 10 (т.к. переданное значение {originalMax} <= 1).");
                }

                // Проверяем порядок 
                if (minDamage > maxDamage) {
                    (minDamage, maxDamage) = (maxDamage, minDamage);
                    Console.WriteLine($"Некорректные входные данные для оружия '{Name}': " +
                                    $"minDamage ({originalMin}) > maxDamage ({originalMax}). " +
                                    $"Значения были поменяны местами.");
                }

                // Создаем новый Interval с корректными значениями
                Damage = new Interval(minDamage, maxDamage);
            }

            // Получение среднего урона
            public int GetAverageDamage() {
                return (Damage.Min + Damage.Max) / 2;
            }

            // Получение случайного урона
            public int GetDamage() {
                return Damage.Get();
            }

            public override string ToString() {
                return $"Оружие: {Name}, Урон: {Damage}, Средний: {GetAverageDamage()}";
            }
        }

        public struct Interval {
            private static readonly Random _random = new Random();
            public int Min { get; }
            public int Max { get; }

            // Конструктор
            public Interval(int minValue, int maxValue) {
                int originalMin = minValue;
                int originalMax = maxValue;

                // Оба числа должны быть >= 0
                if (minValue < 0) {
                    minValue = 0;
                    Console.WriteLine($"Некорректные входные данных: minValue ({originalMin}) < 0. Значение заменено на 0.");
                }

                if (maxValue < 0) {
                    maxValue = 0;
                    Console.WriteLine($"Некорректные входные данных: maxValue ({originalMax}) < 0. Значение заменено на 0.");
                }

                // Если min > max - меняем местами
                if (minValue > maxValue) {
                    (minValue, maxValue) = (maxValue, minValue);
                    Console.WriteLine($"Некорректные входные данных: minValue ({originalMin}) > maxValue ({originalMax}). Значения были поменяны местами.");
                }

                // Если оба числа равны, увеличиваем Max на 10
                if (minValue == maxValue) {
                    maxValue += 10;
                    Console.WriteLine($"Некорректные входные данных: minValue ({minValue}) == maxValue ({minValue}). maxValue увеличено на 10.");
                }

                // Устанавливаем значения свойств
                Min = minValue;
                Max = maxValue;
            }

            // Метод для получения случайного значения в интервале
            public int Get() {
                return _random.Next(Min, Max + 1);
            }

            public override string ToString() {
                return $"[{Min}-{Max}]";
            }
        }

        // Структура Room
        public struct Room {
            public Unit Unit { get; }
            public Weapon Weapon { get; }

            // Конструктор с 2 параметрами
            public Room(Unit unit, Weapon weapon) {
                Unit = unit;
                Weapon = weapon;
            }
        }

        // Класс Dungeon
        public class Dungeon {
            // Поле для массива комнат
            private Room[] _rooms;
            // Метод для получения имени юнита
            private string GetUnitName(int index) {
                string[] unitNames = {
                    "Воин", "Лучник", "Маг", "Варвар", "Рыцарь",
                    "Разбойник", "Некромант", "Паладин", "Друид", "Монах"
                };

                return unitNames[index % unitNames.Length] + " " + (index + 1);
            }

            // Метод для получения имени оружия
            private string GetWeaponName(int index) {
                string[] weaponNames = {
                    "Меч", "Лук", "Посох", "Топор", "Кинжал",
                    "Булава", "Копье", "Арбалет", "Серп", "Цеп"
                };

                return weaponNames[index % weaponNames.Length] + " " + (index + 1);
            }

            // Метод для создания юнита с разными параметрами урона
            private Unit CreateUnit(string name, int index) {
                switch (index % 5) {
                    case 0: return new Unit(name, 2, 8);
                    case 1: return new Unit(name, 5, 12);
                    case 2: return new Unit(name, 8, 16);
                    case 3: return new Unit(name, 1, 6);
                    case 4: return new Unit(name, 10, 20);
                    default: return new Unit(name);
                }
            }

            // Метод для создания оружия с разными параметрами
            private Weapon CreateWeapon(string name, int index) {
                switch (index % 5) {
                    case 0: return new Weapon(name, 5, 15);
                    case 1: return new Weapon(name, 10, 20);
                    case 2: return new Weapon(name, 1, 8);
                    case 3: return new Weapon(name, 8, 12);
                    case 4: return new Weapon(name, 15, 25);
                    default: return new Weapon(name);
                }
            }

            // Конструктор по умолчанию
            public Dungeon() {
                Random random = new Random();
                int roomCount = random.Next(3, 6);

                _rooms = new Room[roomCount];

                // Заполняем массив разными юнитами и оружием
                for (int i = 0; i < roomCount; i++) {
                    string unitName = GetUnitName(i);
                    string weaponName = GetWeaponName(i);

                    Unit unit = CreateUnit(unitName, i);
                    Weapon weapon = CreateWeapon(weaponName, i);

                    _rooms[i] = new Room(unit, weapon);
                }
            }

            // Метод для отображения всех комнат
            public void ShowRooms() {
                Console.WriteLine($"=== ПОДЗЕМЕЛЬЕ (комнат: {_rooms.Length}) ===\n");

                for (int i = 0; i < _rooms.Length; i++) {
                    var room = _rooms[i];

                    Console.WriteLine($"Комната #{i + 1}:");
                    Console.WriteLine($"  {room.Unit}");
                    Console.WriteLine($"  {room.Weapon}");

                    // Дополнительная информация
                    float unitRealHealth = room.Unit.GetRealHealth();
                    Console.WriteLine($"  Фактическое здоровье юнита: {unitRealHealth:F1}");
                    Console.WriteLine($"  Средний урон юнита: {(room.Unit.Damage.Min + room.Unit.Damage.Max) / 2}");
                    Console.WriteLine($"  Средний урон оружия: {room.Weapon.GetAverageDamage()}");

                    // Случайные значения урона для демонстрации
                    Console.WriteLine($"  Тестовый урон юнита: {room.Unit.GetDamage()}");
                    Console.WriteLine($"  Тестовый урон оружия: {room.Weapon.GetDamage()}");

                    Console.WriteLine("—");
                }
            }
        }

        static void Main(string[] args) {
            Console.WriteLine("=== ИГРОВАЯ ЛОГИКА ===\n");

            Dungeon dungeon = new Dungeon();
            dungeon.ShowRooms();

            // Демонстрация работы Interval в отдельности
            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ РАБОТЫ INTERVAL ===\n");

            Console.WriteLine("Тест 1: Interval для оружия");
            Weapon testWeapon = new Weapon("Тестовый меч", 10, 20);
            Console.WriteLine($"Оружие: {testWeapon}");
            Console.WriteLine($"Случайные удары: {testWeapon.GetDamage()}, {testWeapon.GetDamage()}, {testWeapon.GetDamage()}");

            Console.WriteLine("\nТест 2: Interval для юнита");
            Unit testUnit = new Unit("Тестовый воин", 5, 15);
            Console.WriteLine($"Юнит: {testUnit}");
            Console.WriteLine($"Случайные удары: {testUnit.GetDamage()}, {testUnit.GetDamage()}, {testUnit.GetDamage()}");

            Console.WriteLine("\nТест 3: Interval с некорректными значениями");
            Interval testInterval1 = new Interval(20, 10);
            Console.WriteLine($"Интервал [20, 10] после корректировки: {testInterval1}");

            Interval testInterval2 = new Interval(-5, 5);
            Console.WriteLine($"Интервал [-5, 5] после корректировки: {testInterval2}");

            Interval testInterval3 = new Interval(7, 7);
            Console.WriteLine($"Интервал [7, 7] после корректировки: {testInterval3}");

        }
    }
}
