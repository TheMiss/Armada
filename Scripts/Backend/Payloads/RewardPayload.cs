using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public enum RewardRequirementType
    {
        DestroyEnemyGreaterThanOrEquals,
        HasHpGreaterThanOrEquals
    }

    [Exchange]
    public enum RewardType
    {
        GoldShard,
        RedGem,
        EvilHeart,
        Item
    }

    [Exchange]
    [Serializable]
    public class RewardPayload
    {
        public RewardType Type;
        public int ItemSheetId;
        public int Quantity;
    }
}
