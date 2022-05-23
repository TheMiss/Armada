using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class PlayerStatisticsPayload
    {
        public PlayerStatisticOtherPayload[] Stats = { };
        public PlayerStatisticChestPayload[] Chests = { };
        public PlayerStatisticEnemyPayload[] Enemies = { };
    }
}
