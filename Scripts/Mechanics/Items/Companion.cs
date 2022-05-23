using Armageddon.Backend.Payloads;
using Armageddon.Sheets.Items;
using Armageddon.Worlds.Actors.Weapons;
using Cysharp.Threading.Tasks;

namespace Armageddon.Mechanics.Items
{
    public class Companion : Item
    {
        /// <summary>
        ///     Damage per second
        /// </summary>
        // [ShowInInspector]
        public ItemStat DamagePerSecond { get; private set; }

        /// <summary>
        ///     Fires per second
        /// </summary>
        public ItemStat FireRate { get; private set; }

        /// <summary>
        ///     The number of shots per fire
        /// </summary>
        public ItemStat ShotsPerFire { get; private set; }

        /// <summary>
        ///     Damage Per Shot is the result of Damage divided by ShotsPerFire
        /// </summary>
        public ItemStat DamagePerShot { get; private set; }

        public ItemStat CriticalChance { get; private set; }

        public ItemStat CriticalDamage { get; private set; }

        public new CompanionSheet Sheet => (CompanionSheet)base.Sheet;

        protected override async UniTask InitializeStatsAsync(ItemSheet sheet, ItemPayload itemPayload)
        {
            var companionSheet = (CompanionSheet)sheet;
            WeaponActor weaponActor = await companionSheet.WeaponSheet.LoadWeaponAsync();

            DamagePerSecond = new ItemStat(itemPayload.DamagePerSecond);
            FireRate = new ItemStat(itemPayload.FireRate);
            ShotsPerFire = new ItemStat(weaponActor.ShotsPerFire);
            DamagePerShot = new ItemStat(DamagePerSecond / FireRate / ShotsPerFire);
            CriticalChance = new ItemStat(itemPayload.CriticalChance);
            CriticalDamage = new ItemStat(itemPayload.CriticalDamage);
        }
    }
}
