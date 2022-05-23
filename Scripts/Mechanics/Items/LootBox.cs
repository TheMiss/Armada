using Armageddon.Sheets.Items;

namespace Armageddon.Mechanics.Items
{
    public class LootBox : Item
    {
        public new ChestSheet Sheet => (ChestSheet)base.Sheet;
    }
}
