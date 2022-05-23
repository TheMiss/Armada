using System;
using Armageddon.Mechanics.Mails;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;

namespace Armageddon.UI.Common.MailModule
{
    public class MailAttachedItemWidget : Widget
    {
        [SerializeField]
        private ObjectHolderItem m_objectHolderItem;

        [SerializeField]
        private ObjectHolderCurrency m_objectHolderCurrency;

        private ObjectHolderAuto m_objectHolderAuto;

        private ObjectHolderAuto ObjectHolderAuto => m_objectHolderAuto ??=
            new ObjectHolderAuto(m_objectHolderItem, m_objectHolderCurrency);

        public void SetAttachItem(MailAttachedItem attachedItem)
        {
            switch (attachedItem.Type)
            {
                case MailAttachedItemType.Currency:
                    ObjectHolderAuto.SetObject(attachedItem.Currency);
                    break;
                case MailAttachedItemType.Item:
                    ObjectHolderAuto.SetObject(attachedItem.Item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
