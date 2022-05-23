using System;
using System.Globalization;
using Sirenix.OdinInspector;

namespace Armageddon.Sheets
{
    [Serializable]
    public class InShopPrice
    {
        [OnValueChanged(nameof(OnGoldShardChanged))]
        [LabelWidth(80)]
        [HorizontalGroup("Line1")]
        public uint GoldShard;

        [DisplayAsString]
        [LabelWidth(80)]
        [HorizontalGroup("Line2")]
        public uint RedGem;

        [LabelWidth(80)]
        [HorizontalGroup("Line3")]
        public uint EvilHeart;

        [HideLabel]
        [DisplayAsString]
        [EnableGUI]
        [ShowInInspector]
        [HorizontalGroup("Line1")]
        private string FormattedGoldShard => GoldShard.ToString("N0", new CultureInfo("en-US"));

        [HideLabel]
        [DisplayAsString]
        [EnableGUI]
        [ShowInInspector]
        [HorizontalGroup("Line2")]
        private string FormattedRedGem => RedGem.ToString("N0", new CultureInfo("en-US"));

        [HideLabel]
        [DisplayAsString]
        [EnableGUI]
        [ShowInInspector]
        [HorizontalGroup("Line3")]
        private string FormattedEvilHeart => EvilHeart.ToString("N0", new CultureInfo("en-US"));

        private void OnGoldShardChanged()
        {
            RedGem = GoldShard / 50;
        }
    }
}
