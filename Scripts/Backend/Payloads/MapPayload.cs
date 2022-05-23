using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class MapPayload
    {
        public int Id;
        public StagePayload[] MainStages;
        public StagePayload[] SpecialStages;
    }
}
