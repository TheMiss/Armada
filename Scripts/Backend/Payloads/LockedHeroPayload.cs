using System;
using System.Collections.Generic;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class LockedHeroPayload
    {
        public int SheetId;
        public Dictionary<string, uint> Prices;
    }
}
