using System;

namespace Armageddon.Backend.Payloads
{
    [Serializable]
    public class InventoryPayload
    {
        public SlotPayload[] Slots;
    }
}
