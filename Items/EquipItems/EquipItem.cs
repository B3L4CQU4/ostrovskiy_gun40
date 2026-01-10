using GamePrototype.Items.EconomicItems;
using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems {
    public abstract class EquipItem : Item {
        private uint _durability;
        private uint _maxDurability;
        public uint Durability { get => _durability; protected set => _durability = value; }
        public override bool Stackable => false;

        public abstract EquipSlot Slot { get; }
        public virtual uint GetDamage() => 0;
        public virtual uint GetDefence() => 0;
        public virtual bool TakesDamageOnHit => false;
        public virtual bool TakesDamageOnAttack => false;

        protected EquipItem(uint maxDurability, string name) : base(name) {
            _maxDurability = maxDurability;
            _durability = maxDurability;
        }

        public void ReduceDurability(uint delta) {
            _durability = delta >= _durability ? 0 : _durability - delta;
        }

        public void Repair(uint delta) {
            _durability = Math.Min(_durability + delta, _maxDurability);
        }
    }
}
