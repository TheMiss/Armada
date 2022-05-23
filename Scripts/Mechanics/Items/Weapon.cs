using System;
using Armageddon.Backend.Payloads;
using Armageddon.Sheets.Items;
using Armageddon.Worlds.Actors.Weapons;
using Cysharp.Threading.Tasks;

namespace Armageddon.Mechanics.Items
{
    public enum WeaponInstallationType
    {
        Primary,
        Secondary
    }

    public class Weapon : Item
    {
        /// <summary>
        ///     Damage per second
        /// </summary>
        // [ShowInInspector]
        public ItemStat DamagePerSecond { get; set; }

        /// <summary>
        ///     Fires per second
        /// </summary>
        public ItemStat FireRate { get; set; }

        /// <summary>
        ///     The number of shots per fire
        /// </summary>
        public ItemStat ShotsPerFire { get; set; }

        /// <summary>
        ///     Damage Per Shot is the result of Damage divided by ShotsPerFire
        /// </summary>
        public ItemStat DamagePerShot { get; set; }

        public WeaponInstallationType InstallationType { get; set; }

        public new WeaponSheet Sheet
        {
            get => (WeaponSheet)base.Sheet;
            set => base.Sheet = value;
        }

        protected override async UniTask InitializeStatsAsync(ItemSheet sheet, ItemPayload itemPayload)
        {
            var weaponSheet = (WeaponSheet)sheet;
            WeaponActor weaponActor = await weaponSheet.LoadWeaponAsync();

            DamagePerSecond = new ItemStat(itemPayload.DamagePerSecond);
            FireRate = new ItemStat(itemPayload.FireRate);
            ShotsPerFire = new ItemStat(weaponActor.ShotsPerFire);
            DamagePerShot = new ItemStat(DamagePerSecond / FireRate / ShotsPerFire);

            InstallationType = sheet.Type switch
            {
                ItemType.PrimaryWeapon => WeaponInstallationType.Primary,
                ItemType.SecondaryWeapon => WeaponInstallationType.Secondary,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
