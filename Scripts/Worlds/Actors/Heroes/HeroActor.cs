using Armageddon.Worlds.Actors.Characters;
using Armageddon.Worlds.Actors.Unused;
using Armageddon.Worlds.Actors.Weapons;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Heroes
{
    public enum HeroType
    {
        Innatus,
        Faker1,
        Faker2
    }

    /// <summary>
    ///     Base class for all Heroes
    /// </summary>
    [RequireComponent(typeof(Damageable))]
    public class HeroActor : CharacterActor
    {
        public const int MaxFirePowerStatValue = 10;

        public override void Initialize(CharacterDescriptor descriptor)
        {
            base.Initialize(descriptor);

            AutoCastSkills = false;
            // Damager.gameObject.SetActive(false);

            CanTick = true;
            // Hud.gameObject.SetActive(false);
        }

        public void SetPrimaryWeapon(WeaponActor weapon, bool autoFire = true)
        {
            AttachWeapon(weapon, 0);
            weapon.IsFiring = autoFire;
        }

        public void SetSecondaryWeapon(WeaponActor weapon, bool autoFire = true)
        {
            AttachWeapon(weapon, 1);
            weapon.IsFiring = autoFire;
        }
    }
}
