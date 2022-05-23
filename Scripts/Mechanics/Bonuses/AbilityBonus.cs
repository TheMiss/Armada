using Armageddon.Sheets.Abilities;
using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Mechanics.Bonuses
{
    public class AbilityBonus : Bonus
    {
        public AbilityBonus(object source, AbilitySheet sheet, int level, int minLevel, int maxLevel)
            : base(BonusType.Ability)
        {
            Source = source;
            Sheet = sheet;
            Level = level;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }

        public object Source { get; }

        public AbilitySheet Sheet { get; }

        [TableColumnWidth(170)]
        [ShowAsString]
        public string Name => Sheet.Name;

        [ShowAsString]
        public int Level { get; }

        public int MinLevel { get; }
        public int MaxLevel { get; }
    }
}
