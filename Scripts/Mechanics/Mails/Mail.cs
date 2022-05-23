using System;
using System.Collections.Generic;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Items;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Mechanics.Mails
{
    [Exchange]
    public enum MailAttachedItemType
    {
        Currency,
        Item
    }

    public class MailAttachedItem
    {
        public MailAttachedItem(Item item)
        {
            Type = MailAttachedItemType.Item;
            Item = item;
        }

        public MailAttachedItem(Currency currency)
        {
            Type = MailAttachedItemType.Currency;
            Currency = currency;
        }

        public MailAttachedItemType Type { get; }
        public Item Item { get; }
        public Currency Currency { get; }
    }

    [Serializable]
    public class Mail : IBadge
    {
        public string InstanceId { get; private set; }
        public MailType Type { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public DateTime ReceivedDateTime { get; private set; }
        public DateTime ExpiredDateTime { get; private set; }
        public List<MailAttachedItem> AttachedItems { get; } = new();
        public bool IsNoticed { get; set; }
        
        public override string ToString()
        {
            return $"{Title} ({InstanceId})";
        }

        public async UniTask InitializeAsync(MailPayload payload)
        {
            InstanceId = payload.InstanceId;
            Type = payload.Type;
            Title = payload.Title;
            Message = payload.Message;
            ReceivedDateTime = payload.ReceivedDateTime;
            ExpiredDateTime = payload.ExpiredDateTime;

            foreach (MailAttachedItemPayload attachedItemPayload in payload.AttachedItems)
            {
                switch (attachedItemPayload.Type)
                {
                    case MailAttachedItemType.Item:
                    {
                        if (attachedItemPayload.Item == null)
                        {
                            Debug.LogError($"type is {MailAttachedItemType.Item}, but the data is null.");
                            continue;
                        }

                        var item = await Item.CreateAsync(attachedItemPayload.Item);
                        var attachedItem = new MailAttachedItem(item);

                        AttachedItems.Add(attachedItem);
                        break;
                    }
                    case MailAttachedItemType.Currency:
                    {
                        if (attachedItemPayload.Currency == null)
                        {
                            Debug.LogError($"type is {MailAttachedItemType.Currency}, but the data is null.");
                            continue;
                        }

                        var currency = new Currency(attachedItemPayload.Currency);
                        var attachedItem = new MailAttachedItem(currency);

                        AttachedItems.Add(attachedItem);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await UniTask.CompletedTask;
        }
    }
}
