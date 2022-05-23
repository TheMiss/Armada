using Armageddon.Mechanics.Inventories;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class ObjectHolderSign : ObjectHolder
    {
        [SerializeField]
        private Sprite m_addSprite;

        public void Initialize(SignData signData)
        {
            if (signData.Type == SignType.Add)
            {
                SetIcon(m_addSprite);
            }
            else
            {
                Debug.LogError($"Not implemented {signData} yet!");
            }
        }

        public static ObjectHolderSign Create(IInventoryObject obj,
            ObjectHolder objectHolderPrefab)
        {
            if (!(obj is SignData signData))
            {
                Debug.LogWarning($"slotObject is {obj}");
                return null;
            }

            var itemHolder = (ObjectHolderSign)Instantiate(objectHolderPrefab);
            itemHolder.name = $"{itemHolder.name}({signData.Type})";
            itemHolder.Initialize(signData);
            itemHolder.AllowDrag = true;
            // itemHolder.AllowSelect = true;
            itemHolder.Object = obj;

            return itemHolder;
        }
    }
}
