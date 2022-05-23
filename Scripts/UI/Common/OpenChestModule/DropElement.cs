using System;
using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon.UI.Common.OpenChestModule
{
    public class DropElement : SelectableElement<DropElement>
    {
        [SerializeField]
        private ObjectHolderItem m_objectHolderItem;

        [SerializeField]
        private ObjectHolderCurrency m_objectHolderCurrency;

        private ObjectHolderAuto m_objectHolderAuto;

        private ObjectHolderAuto ObjectHolderAuto => m_objectHolderAuto ??=
            new ObjectHolderAuto(m_objectHolderItem, m_objectHolderCurrency);

        public override object Object => ObjectHolderAuto.Object;

        private void Initialize(Item item)
        {
            ObjectHolderAuto.SetObject(item);
        }

        private void Initialize(Currency currency)
        {
            ObjectHolderAuto.SetObject(currency);
        }

        public static async UniTask<List<DropElement>> CreateManyAsync(DropPayload[] dropPayloads,
            DropElement dropElementPrefab, RectTransform parentTransform, bool clearOldContent = true)
        {
            if (clearOldContent)
            {
                parentTransform.DestroyChildren();
            }

            var dropElements = new List<DropElement>();

            foreach (DropPayload dropPayload in dropPayloads)
            {
                DropElement dropElement = Instantiate(dropElementPrefab,
                    parentTransform);

                dropElement.gameObject.SetActive(false);
                dropElements.Add(dropElement);

                switch (dropPayload.Type)
                {
                    case DropType.Item:
                    {
                        var item = await Item.CreateAsync(dropPayload.Item);
                        dropElement.Initialize(item);
                        break;
                    }
                    case DropType.Currency:
                        var currency = new Currency(dropPayload.Currency);
                        dropElement.Initialize(currency);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return dropElements;
        }
    }
}
