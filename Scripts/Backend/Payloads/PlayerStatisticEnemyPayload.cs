using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class PlayerStatisticEnemyPayload
    {
        public int Id;
        public int Killed;
    }
}
