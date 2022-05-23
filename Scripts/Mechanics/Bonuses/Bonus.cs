using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Mechanics.Bonuses
{
    public abstract class Bonus
    {
        [TableColumnWidth(60, false)]
        [ShowAsString]
        public readonly BonusType Type;

        protected Bonus(BonusType type)
        {
            Type = type;
        }
    }
}
