using System;
using Armageddon.Backend.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Payloads
{
    public static class CurrencyCode
    {
        public const string Energy = "EN";
        public const string GoldShard = "GS";
        public const string EvilHeart = "EH";
        public const string RedGem = "RG";
        public const string RealMoney = "RM";
    }

    [Exchange]
    public enum CurrencyType
    {
        Energy,
        GoldShard,
        RedGem,
        EvilHeart,
        RealMoney
    }

    public static class CurrencyTypeExtensions
    {
        public static string ToCurrencyCode(this CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Energy => CurrencyCode.Energy,
                CurrencyType.GoldShard => CurrencyCode.GoldShard,
                CurrencyType.RedGem => CurrencyCode.RedGem,
                CurrencyType.EvilHeart => CurrencyCode.EvilHeart,
                CurrencyType.RealMoney => CurrencyCode.RealMoney,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static string ToDisplayName(this CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Energy => "Energy",
                CurrencyType.GoldShard => "Gold Shard",
                CurrencyType.RedGem => "Red Gem",
                CurrencyType.EvilHeart => "Evil Heart",
                CurrencyType.RealMoney => "Real Money",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static string ToSpriteCode(this CurrencyType type, uint amount)
        {
            string text = $"<sprite name=\"{type.ToCurrencyCode()}\"><color=white>{amount}</color>";
            return text;
        }
    }

    [Exchange]
    [Serializable]
    public class CurrencyPayload
    {
        // Actually, since we don't use TypeScript anymore, so we don't need to do this.
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType Type;

        public int Amount;
    }
}
