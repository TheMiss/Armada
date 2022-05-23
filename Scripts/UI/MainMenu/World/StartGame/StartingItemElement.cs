using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class StartingItemElement : SelectableElement<StartingItemElement>
    {
        [SerializeField]
        private ObjectHolderItem m_objectHolderItem;

        [SerializeField]
        private Toggle m_useToggle;

        public Item Item { get; private set; }

        public override object Object => Item;

        public Toggle UseToggle => m_useToggle;

        public void Initialize(Item item)
        {
            Item = item;

            m_objectHolderItem.name = $"{m_objectHolderItem.name})";
            m_objectHolderItem.Initialize(item);
        }
    }

    // public class StartingItemElement : Purity.UI.Widget, ISelectHandler, IDeselectHandler
    // {
    //     [SerializeField]
    //     private ObjectHolderItem m_objectHolderItem;
    //
    //     [SerializeField]
    //     private Toggle m_useToggle;
    //
    //     public Item Item { get; private set; }
    //
    //     public Toggle UseToggle => m_useToggle;
    //     
    //     public UnityEvent<StartingItemElement> Selected { get; set; } = new UnityEvent<StartingItemElement>();
    //     public UnityEvent<StartingItemElement> Deselected { get; set; } = new UnityEvent<StartingItemElement>();
    //
    //     public void Initialize(Item item)
    //     {
    //         Item = item;
    //         
    //         m_objectHolderItem.name = $"{m_objectHolderItem.name})";
    //         m_objectHolderItem.Initialize(item);
    //     }
    //     
    //     void ISelectHandler.OnSelect(BaseEventData eventData)
    //     {
    //         Debug.Log($"OnSelect {Item.Name}");
    //         
    //         Selected?.Invoke(this);
    //     }
    //
    //     void IDeselectHandler.OnDeselect(BaseEventData eventData)
    //     {
    //         Debug.Log($"OnDeselect {Item.Name}");
    //         
    //         Deselected?.Invoke(this);
    //     }
    // }
}
