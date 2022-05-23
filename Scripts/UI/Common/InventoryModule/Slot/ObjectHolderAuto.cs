using Armageddon.Mechanics;
using Armageddon.Mechanics.Items;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class ObjectHolderAuto
    {
        private readonly ObjectHolderItem m_holderItem;
        private readonly ObjectHolderCurrency m_holderCurrency;

        public ObjectHolderAuto(ObjectHolderItem holderItem, ObjectHolderCurrency holderCurrency)
        {
            m_holderItem = holderItem;
            m_holderCurrency = holderCurrency;
        }

        public object Object { get; private set; }

        private void SetObjects()
        {
            m_holderItem.gameObject.SetActive(false);
            m_holderCurrency.gameObject.SetActive(false);
        }

        public void SetObject(Item item)
        {
            SetObjects();

            Object = item;
            m_holderItem.gameObject.SetActive(true);
            m_holderItem.name = $"{m_holderItem.name})";
            m_holderItem.Initialize(item);
        }

        public void SetObject(Currency currency)
        {
            SetObjects();

            Object = currency;
            m_holderCurrency.gameObject.SetActive(true);
            m_holderCurrency.SetCurrency(currency);
        }
    }
}
