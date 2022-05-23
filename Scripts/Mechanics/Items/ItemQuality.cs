using System;
using Armageddon.Backend.Attributes;
using UnityEngine;

namespace Armageddon.Mechanics.Items
{
    [Exchange]
    public enum ItemQuality
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Immortal,
        Ancient
    }

    public static class ItemQualityExtensions
    {
        public static Color ToColor(this ItemQuality itemQuality)
        {
            return itemQuality switch
            {
                ItemQuality.Common => new Color(255 / 255f, 255 / 255f, 240 / 255f),
                ItemQuality.Uncommon => new Color(61 / 255f, 210 / 255f, 11 / 255f),
                ItemQuality.Rare => new Color(47 / 255f, 120 / 255f, 255 / 255f),
                ItemQuality.Epic => new Color(230 / 255f, 65 / 255f, 255 / 255f),
                ItemQuality.Legendary => new Color(255 / 255f, 180 / 255f, 0 / 255f),
                ItemQuality.Immortal => new Color(0 / 255f, 255 / 255f, 255 / 255f),
                // ItemQuality.Ancient => new Color(255/255f,50/255f,50/255f),
                ItemQuality.Ancient => new Color(235 / 255f, 75 / 255f, 75 / 255f),
                _ => throw new ArgumentOutOfRangeException(nameof(itemQuality), itemQuality, null)
            };
        }
    }
}
