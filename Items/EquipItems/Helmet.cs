using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems {
    public sealed class Helmet : EquipItem {
        public uint Defence { get; }

        public Helmet(uint defence, uint durability, string name)
            : base(durability, name) {
            Defence = defence;
        }

        public override uint GetDefence() => Defence;

        public override EquipSlot Slot => EquipSlot.Helmet;
    }
}
