using Armageddon.Backend.Payloads;
using Armageddon.Sheets.Items;
using Cysharp.Threading.Tasks;

namespace Armageddon.Mechanics.Items
{
    public class Armor : Item
    {
        public ItemStat ArmorBonus { get; private set; }

        protected override async UniTask InitializeStatsAsync(ItemSheet sheet, ItemPayload itemPayload)
        {
            ArmorBonus = new ItemStat(itemPayload.Armor);
            await UniTask.CompletedTask;
        }
    }
}
