// See https://aka.ms/new-console-template for more information
using System;


namespace MyApp {

    class Program {

        class Unit {
            // Имя
            public string Name { get; }

            // Здоровье
            private float _health;
            public float Health => _health; 

            // Урон
            public int Damage { get; } 
        
            // Броня
            public float Armor { get; } 

            // Конструкторы
            public Unit() : this("Unknown Unit") {}

            public Unit(string name) {
                Name = name;
                Damage = 5;
                Armor = 0.6f;
                _health = 100f;
            }

            // Получение фактического здоровья
            public float GetRealHealth() {
                return _health * (1f + Armor);
            }

            // Получение урона
            public bool SetDamage(float value)
            {
                _health -= value * Armor;
                return _health <= 0f;
            }
        }

        class Weapon {
            // Имя
            public string Name { get; }

            // Минимальный урон
            public int MinDamage { get; private set; }

            // Максимальный урон
            public int MaxDamage { get; private set; }

            // Прочность
            private float _Durability;

            // Конструкторы
            public Weapon(string name) {
                Name = name;
                _Durability = 1f;
                MinDamage = 1;
                MaxDamage = 10;
            }

            public Weapon(string name, int minDamage, int maxDamage) : this(name) {
                SetDamageParams(minDamage, maxDamage);
            }

            // Установка значений
            public void SetDamageParams(int minDamage, int maxDamage)
            {
                int originalMin = minDamage;
                int originalMax = maxDamage;
                
                // Корректируем отдельные значения
                if (minDamage < 1)
                {
                    minDamage = 1;
                    Console.WriteLine($"Для оружия '{Name}' выполнена форсированная установка минимального значения урона: 1.");
                }
                
                if (maxDamage <= 1)
                {
                    maxDamage = 10;
                    Console.WriteLine($"Для оружия '{Name}' установлено максимальное значение урона: 10 (т.к. переданное значение {originalMax} <= 1).");
                }
                
                // Проверяем порядок 
                if (minDamage > maxDamage)
                {
                    (minDamage, maxDamage) = (maxDamage, minDamage);
                    Console.WriteLine($"Некорректные входные данные для оружия '{Name}': " +
                                    $"minDamage ({originalMin}) > maxDamage ({originalMax}). " +
                                    $"Значения были поменяны местами.");
                }
                
                MinDamage = minDamage;
                MaxDamage = maxDamage;
            }
            public int GetDamage()
            {
                return (MinDamage + MaxDamage) / 2;
            }
        }
        static void Main(string[] args) {
            // Тесты SetDamageParams
            /*
            Console.WriteLine("");
            Weapon axe = new Weapon("Топор", 20, 10);
            Console.WriteLine(
                $"Оружие: {axe.Name}, Min: {axe.MinDamage}, Max: {axe.MaxDamage}, Средний: {axe.GetDamage()}"
            );

            Console.WriteLine("");
            Console.WriteLine("");

            // Тест с minDamage < 1
            Weapon dagger = new Weapon("Кинжал", -5, 8);
            Console.WriteLine(
                $"Оружие: {dagger.Name}, Min: {dagger.MinDamage}, Max: {dagger.MaxDamage}, Средний: {dagger.GetDamage()}"
            );

            Console.WriteLine("");
            Console.WriteLine("");
            
            // Тест с maxDamage <= 1
            Weapon staff = new Weapon("Посох", 3, 1);
            Console.WriteLine(
                $"Оружие: {staff.Name}, Min: {staff.MinDamage}, Max: {staff.MaxDamage}, Средний: {staff.GetDamage()}"
            );

            Console.WriteLine("");
            Console.WriteLine("");
            
            // Тест всех проверок сразу
            Weapon weirdWeapon = new Weapon("Странное оружие", 20, -5);
            Console.WriteLine(
                $"Оружие: {weirdWeapon.Name}, Min: {weirdWeapon.MinDamage}, Max: {weirdWeapon.MaxDamage}, Средний: {weirdWeapon.GetDamage()}"
            );
            */
        }
    }
}
