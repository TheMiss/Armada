using Armageddon.Backend.Payloads;
using Armageddon.Sheets.Items;
using Cysharp.Threading.Tasks;

namespace Armageddon.Mechanics.Items
{
    public class Kernel : Item
    {
        public ItemStat DexterityMultiplier { get; private set; }

        public ItemStat VitalityMultiplier { get; private set; }

        public ItemStat PerceptionMultiplier { get; private set; }

        public ItemStat LeadershipMultiplier { get; private set; }

        protected override async UniTask InitializeStatsAsync(ItemSheet sheet, ItemPayload itemPayload)
        {
            // if (stats.Length != 5)
            // {
            //     Debug.LogError($"Kernel: stats.Length == {stats.Length}");
            //     return;
            // }

            DexterityMultiplier = new ItemStat(itemPayload.DexterityMultiplier);
            VitalityMultiplier = new ItemStat(itemPayload.VitalityMultiplier);
            PerceptionMultiplier = new ItemStat(itemPayload.PerceptionMultiplier);
            LeadershipMultiplier = new ItemStat(itemPayload.LeadershipMultiplier);

            await UniTask.CompletedTask;
        }
    }
}
