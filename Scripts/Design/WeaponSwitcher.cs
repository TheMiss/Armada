using System.Collections.Generic;
using System.Linq;
using Armageddon.Worlds.Actors.Heroes;
using Armageddon.Worlds.Actors.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Design
{
    public class WeaponSwitcher : SandboxContext
    {
        [HideInEditorMode]
        public PlayerController PlayerController;

        [OnValueChanged(nameof(OnPrimaryWeaponChanged))]
        public WeaponActor PrimaryWeapon;

        [OnValueChanged(nameof(OnSecondaryWeaponChanged))]
        public WeaponActor SecondaryWeapon;

        [HideInEditorMode]
        [ShowInInspector]
        private List<WeaponActor> Weapons { get; set; }

        public void Initialize()
        {
            // OnPrimaryWeaponChanged();
            // OnSecondaryWeaponChanged();

            PlayerController = GetService<PlayerController>();

            Weapons = GetComponentsInChildren<WeaponActor>(true).ToList();

            foreach (WeaponActor weapon in Weapons)
            {
                weapon.gameObject.SetActive(false);
                weapon.Initialize();
                weapon.IsFiring = false;
            }
        }

        private void OnPrimaryWeaponChanged()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (PrimaryWeapon == null)
            {
                Debug.LogError($"{nameof(PrimaryWeapon)} is null. What are you trying to do chap?");
                return;
            }

            WeaponActor primaryWeapon = Instantiate(PrimaryWeapon);
            primaryWeapon.gameObject.SetActive(true);

            HeroActor heroActor = World.HeroActor;
            heroActor.SetPrimaryWeapon(primaryWeapon);
        }

        private void OnSecondaryWeaponChanged()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (SecondaryWeapon == null)
            {
                Debug.LogError($"{nameof(SecondaryWeapon)} is null. What are you trying to do chap?");
                return;
            }

            WeaponActor subWeapon = Instantiate(SecondaryWeapon);
            subWeapon.gameObject.SetActive(true);

            HeroActor heroActor = World.HeroActor;
            heroActor.SetSecondaryWeapon(subWeapon);
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
            }
        }
    }
}
