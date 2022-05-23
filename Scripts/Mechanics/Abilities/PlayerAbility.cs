using Armageddon.Sheets;
using Armageddon.Sheets.Abilities;

namespace Armageddon.Mechanics.Abilities
{
    public class PlayerAbility : Ability
    {
        public PlayerAbility(object source, AbilitySheet sheet, int level) : base(source, sheet, level)
        {
        }

        public int UpgradeableLevel { get; set; }
        public int MaxUpgradeableLevel { get; set; }

        public new PlayerAbilitySheet Sheet => (PlayerAbilitySheet)base.Sheet;

        public bool HasNextUpgrade => Level < Sheet.UpgradeDetailsRows.Count;

        public Price UpgradePrice => Sheet.UpgradeDetailsRows[Level].Price;
    }
}
