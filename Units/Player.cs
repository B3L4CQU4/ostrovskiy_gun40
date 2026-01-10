using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Utils;
using System.Text;

namespace GamePrototype.Units {
    public sealed class Player : Unit {
        private readonly Dictionary<EquipSlot, EquipItem> _equipment = new();

        public Player(string name, uint health, uint maxHealth, uint baseDamage) : base(name, health, maxHealth, baseDamage) {
        }

        public override uint GetUnitDamage() {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item)) {
                return BaseDamage + item.GetDamage();
            }
            return BaseDamage;
        }

        public override void HandleCombatComplete() {
        }

        public override void AddItemToInventory(Item item) {
            if (item is EquipItem equipItem && _equipment.TryAdd(equipItem.Slot, equipItem)) {
                // Item was equipped
                return;
            }
            base.AddItemToInventory(item);
        }

        private void UseEconomicItem(EconomicItem economicItem) {
            if (economicItem is HealthPotion healthPotion) {
                Health = Math.Min(MaxHealth, Health + healthPotion.HealthRestore);
            }
        }

        protected override uint CalculateAppliedDamage(uint damage) {
            if (_equipment.TryGetValue(EquipSlot.Armour, out var item) && item is Armour armour) {
                damage -= (uint)(damage * (armour.Defence / 100f));
            }
            return damage;
        }
        protected override void DamageReceiveHandler() {
            base.DamageReceiveHandler();
            // Копируем список слотов экипировки
            var slots = _equipment.Keys.ToList();
            foreach (var slot in slots) {
                var item = _equipment[slot];
                // пропускаем оружие т.к. по умолчанию TakesDamageOnHit = false
                if (!item.TakesDamageOnHit)
                    continue;
                item.ReduceDurability(1);
                if (item.Durability == 0) {
                    Console.WriteLine($"{item.Name} предмет сломан!");
                    _equipment.Remove(slot);
                } else {
                    Console.WriteLine($"{item.Name} durability - {item.Durability}");
                }
            }
        }

        protected override void OnAttack() {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item)
                && item.TakesDamageOnAttack) {

                item.ReduceDurability(1);

                if (item.Durability == 0) {
                    Console.WriteLine($"{item.Name} сломалось!");
                    _equipment.Remove(EquipSlot.Weapon);
                } else {
                    Console.WriteLine($"{item.Name} durability - {item.Durability}");
                }
            }
        }

        public bool TryUseGrindstone() {
            if (!_equipment.TryGetValue(EquipSlot.Weapon, out var item)
                || item is not Weapon weapon) {
                Console.WriteLine("You have no weapon equipped.");
                return false;
            }

            if (!Inventory.TryTakeFirst<Grindstone>(out var grindstone)) {
                return false;
            }

            var before = weapon.Durability;
            weapon.Repair(grindstone.RepairAmount);

            Console.WriteLine(
                $"{weapon.Name} repaired: {before} → {weapon.Durability}"
            );

            return true;
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine(Name);
            builder.AppendLine($"Health {Health}/{MaxHealth}");

            builder.AppendLine("Equipment:");
            if (_equipment.Count == 0) {
                builder.AppendLine("  (empty)");
            } else {
                foreach (var (slot, item) in _equipment) {

                    builder.Append($"  {slot}: {item.Name} ");

                    var defence = item.GetDefence();
                    if (defence > 0) {
                        builder.Append($"[Defence {defence}] ");
                    }

                    var damage = item.GetDamage();
                    if (damage > 0) {
                        builder.Append($"[Damage +{damage}] ");
                    }

                    builder.AppendLine($"(Durability {item.Durability})");
                }
            }

            builder.AppendLine("Loot:");
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++) {
                builder.AppendLine($"{i}. [{items[i].Name}] : {items[i].Amount}");
            }

            return builder.ToString();
        }

        public bool TryUseHealthPotion() {
            if (!Inventory.TryTakeFirst<HealthPotion>(out var potion))
                return false;

            UseEconomicItem(potion);
            return true;
        }

        public bool TryEquipFromInventory(int index) {
            var items = Inventory.Items;

            if (index < 0 || index >= items.Count) {
                Console.WriteLine("Invalid inventory index.");
                return false;
            }

            var item = items[index];

            if (item is not EquipItem equipItem) {
                Console.WriteLine($"{item.Name} cannot be equipped.");
                return false;
            }

            // Снять предмет из слота, если есть
            if (_equipment.TryGetValue(equipItem.Slot, out var oldItem)) {
                _equipment.Remove(equipItem.Slot);

                if (!Inventory.TryAdd(oldItem)) {
                    Console.WriteLine("Inventory is full. Cannot unequip item.");
                    // возвращаем всё назад
                    _equipment[equipItem.Slot] = oldItem;
                    return false;
                }

                Console.WriteLine($"{oldItem.Name} unequipped.");
            }

            // Удаляем новый предмет из инвентаря
            if (!Inventory.TryRemove(item)) {
                Console.WriteLine("Failed to remove item from inventory.");
                return false;
            }

            // Экипируем
            _equipment[equipItem.Slot] = equipItem;

            Console.WriteLine($"{equipItem.Name} equipped in slot {equipItem.Slot}.");
            return true;
        }
    }
}
