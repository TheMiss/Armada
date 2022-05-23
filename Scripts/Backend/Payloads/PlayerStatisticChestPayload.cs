using System;
using Armageddon.Backend.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public enum PlayerStatisticChestType
    {
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Immortal,
        Ancient
    }

    [Exchange]
    [Serializable]
    public class PlayerStatisticChestPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerStatisticChestType Type;

        public int Obtained;
        public int Opened;
    }
}
