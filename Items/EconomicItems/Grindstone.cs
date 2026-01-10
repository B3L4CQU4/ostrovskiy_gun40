namespace GamePrototype.Items.EconomicItems {
    public sealed class Grindstone : EconomicItem {
        public uint RepairAmount { get; } = 5;
        public override bool Stackable => false;

        public Grindstone(string name) : base(name) {
        }
    }
}
