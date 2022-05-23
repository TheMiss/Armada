using System;
using System.Collections.Generic;

namespace Armageddon.Backend.Payloads.Local
{
    [Serializable]
    public class PlayerLocalProfile
    {
        public int CurrentMapId;
        public List<string> UsingItemInstances = new();
    }
}
