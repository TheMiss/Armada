using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class HeroInventoryPayload
    {
        public SlotPayload[] Slots;
    }
}
