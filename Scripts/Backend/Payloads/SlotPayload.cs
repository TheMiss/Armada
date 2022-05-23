using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class SlotPayload
    {
        public string InstanceId;
    }
}
