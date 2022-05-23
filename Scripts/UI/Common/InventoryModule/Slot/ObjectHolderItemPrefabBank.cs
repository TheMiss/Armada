using System.Collections.Generic;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class ObjectHolderItemPrefabBank : ScriptableObject
    {
        [SerializeField]
        private List<Sprite> m_itemTypeIcons;

        public IReadOnlyList<Sprite> ItemTypeIcons => m_itemTypeIcons;
    }
}
