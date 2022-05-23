using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public enum PlayerStatisticOtherType
    {
        Other1 = 1,
        Other2 = 2,
        Other3 = 3,
        Other4 = 4,
        Other5 = 5
    }

    [Exchange]
    [Serializable]
    public class PlayerStatisticOtherPayload
    {
        public PlayerStatisticOtherType Type;
        public float Value;
    }
}
