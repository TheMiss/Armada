using System.Collections.Generic;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class UseItemRequest : BackendRequest
    {
        public string ItemInstanceId;
        public int Quantity;
    }

    [FunctionReply]
    public class UseItemReply : BackendReply
    {
        public ItemPayload[] UpdatedUsedItems = { };
        public int? AddedCardPowerId;
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
        public DropPayload[] Drops = { };
    }

    public static class UseItemReplyExtensions
    {
        public static ItemPayload[] GetItemsFromDrops(this UseItemReply reply)
        {
            var items = new List<ItemPayload>();

            foreach (DropPayload lootEntry in reply.Drops)
            {
                if (lootEntry.Type == DropType.Item)
                {
                    items.Add(lootEntry.Item);
                }
            }

            return items.ToArray();
        }
    }
}
