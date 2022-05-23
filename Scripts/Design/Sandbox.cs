using Armageddon.UI.Common;
using Armageddon.Worlds;
using Armageddon.Worlds.Actors.Characters;
using CodeStage.AdvancedFPSCounter;
using Purity.Bullet2D.Shots;
using Purity.Bullet2D.Utilities;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Design
{
    public class Sandbox : Context
    {
        public enum HeroActorType
        {
            Innatus
        }

        [BoxGroup("Prefabs")]
        public World WorldPrefab;

        [BoxGroup("Prefabs")]
        public PlayerController PlayerControllerPrefab;

        [BoxGroup("Prefabs")]
        public GameObject DependenciesPrefab;

        public int MapId = 1;

        public int ZoneId = 1;

        [HideInEditorMode]
        public PlayerController PlayerController;

        public HeroActorType SelectedHeroActorType;

        [InlineEditor]
        public HeroModifier HeroModifier;

        public ItemFactory ItemFactory;

        public WeaponSwitcher WeaponSwitcher;

        public EnemySpawner EnemySpawner;

        [HideInEditorMode]
        public World World;

        [ReadOnly]
        public int CurrentWeaponLevel;

        public int SelectedHeroActorInt => (int)SelectedHeroActorType + 1;

        [ReadOnly]
        [ShowInInspector]
        public CharacterActor MainCharacterActor { private set; get; }

        protected override async void Awake()
        {
            // Create systems from prefabs that we saved from Main scene
            World = Instantiate(WorldPrefab);
            PlayerController = Instantiate(PlayerControllerPrefab);
            Instantiate(DependenciesPrefab);

            AFPSCounter.Instance.deviceInfoCounter.Enabled = false;

            RegisterService(ItemFactory);
            RegisterService(HeroModifier);

            // Forge the fake one
            // Player player = await ArmoryUtility.ForgePlayer();

            await World.AddMainHeroActorAsync(SelectedHeroActorInt);
            await EnemySpawner.SpawnDefaults();
            await World.LoadZoneAsync(MapId, ZoneId);
            World.Prepare();

            HeroModifier.HeroActor = World.HeroActor;
            HeroModifier.Initialize();
            // WeaponSwitcher.Initialize();

            Bullet2DManager.Instance.GetComponent<OnScreenDisplay>().enabled = false;

            // Application.targetFrameRate = 30;
            // CanTick = true;
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // SetWeaponLevel(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // SetWeaponLevel(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // SetWeaponLevel(3);
            }
        }
    }
}
