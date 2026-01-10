using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems {
    public sealed class Pants : EquipItem {
        public uint Defence { get; }

        public Pants(uint defence, uint durability, string name)
            : base(durability, name) {
            Defence = defence;
        }

        public override uint GetDefence() => Defence;

        public override EquipSlot Slot => EquipSlot.Pants;
    }
}
