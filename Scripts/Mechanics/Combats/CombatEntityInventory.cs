using System.Collections;
using System.Collections.Generic;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;

namespace Armageddon.Mechanics.Combats
{
    public class CombatEntityInventory : IEnumerable
    {
        private readonly List<Item> m_items = new() { null, null, null, null, null, null, null, null };

        public Item this[EquipmentSlotType slotType]
        {
            get => m_items[(int)slotType];
            set => m_items[(int)slotType] = value;
        }

        public IEnumerator GetEnumerator()
        {
            return m_items.GetEnumerator();
        }
    }
}
