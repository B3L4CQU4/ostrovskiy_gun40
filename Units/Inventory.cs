using GamePrototype.Items.EconomicItems;

namespace GamePrototype.Units {
    public sealed class Inventory {
        private readonly uint _capacity;
        private readonly List<Item> _items = new List<Item>();
        public IReadOnlyList<Item> Items => _items;

        public Inventory(uint capacity) => _capacity = capacity;

        public bool TryAdd(Item item) {
            if (_items.Count == _capacity) {
                return false;
            }

            _items.Add(item);
            return true;
        }

        public bool TryRemove(Item item) {
            if (_items.Count == 0 || !_items.Contains(item)) {
                return false;
            }
            _items.Remove(item);
            return true;
        }
        public bool TryTakeFirst<T>(out T found) where T : Item {
            found = null;

            for (int i = 0; i < _items.Count; i++) {
                if (_items[i] is T item) {
                    found = item;
                    _items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }
}
