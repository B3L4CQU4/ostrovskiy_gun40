using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Units;

namespace GamePrototype.Utils {
    public class UnitFactoryDemo {
        public static Player CreatePlayer(string name) {
            var player = new Player(name, 30, 30, 6);
            player.AddItemToInventory(new Weapon(10, 20, "Sword"));
            player.AddItemToInventory(new Armour(10, 20, "Armour"));
            player.AddItemToInventory(new HealthPotion("Potion"));
            player.AddItemToInventory(new Grindstone("Grindstone"));
            return player;
        }

        public static Unit CreateGoblinEnemy() => new Goblin(GameConstants.Goblin, 100, 100, 2);
    }
}
