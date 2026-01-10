using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems {
    public sealed class RangeWeapon : EquipItem {
        public override bool TakesDamageOnAttack => true;
        public uint Damage { get; }
        public RangeWeapon(uint damage, uint durability, string name)
            : base(durability, name) {
            Damage = damage;
        }
        public override uint GetDamage() => Damage;
        public override EquipSlot Slot => EquipSlot.Weapon;
    }
}