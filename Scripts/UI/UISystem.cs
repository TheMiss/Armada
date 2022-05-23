using Armageddon.Assistance.BackendDrivers;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Armageddon.UI.Common;
using Armageddon.UI.Common.AccountModule;
using Armageddon.UI.Common.ItemInspectionModule;
using Armageddon.UI.Common.MailModule;
using Armageddon.UI.Common.OpenChestModule;
using Armageddon.UI.Common.SettingsModule;
using Armageddon.UI.Common.ShopModule;
using Armageddon.UI.InGameMenu;
using Armageddon.UI.Unused;
using CodeStage.AdvancedFPSCounter;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI
{
    [DisallowMultipleComponent]
    public class UISystem : UISystemBase
    {
        [SerializeField]
        private Canvas m_topmostCanvas;

        [SerializeField]
        private RectTransform m_commonTransform;

        [SerializeField]
        private BlockerManager m_blockerManager;

        [SerializeField]
        private AlertDialog m_alertDialog;

        [SerializeField]
        private WaitForRetryDialog m_waitForRetryDialog;

        [SerializeField]
        private ShopConfirmDialog m_shopConfirmDialog;

        [SerializeField]
        private UnlockHeroConfirmDialog m_unlockHeroConfirmDialog;

        [SerializeField]
        private ProgressDialog m_progressDialog;

        [SerializeField]
        private WaitForServerResponse m_waitForServerResponse;

        [SerializeField]
        private InspectItemWindow m_inspectItemWindow;

        [SerializeField]
        private CompareItemWindow m_compareItemWindow;
        
        [SerializeField]
        private OpenChestWindow m_openChestWindow;

        [SerializeField]
        private AccountWindow m_accountWindow;

        [SerializeField]
        private ShopWindow m_shopWindow;

        [SerializeField]
        private MailboxWindow m_mailboxWindow;

        [SerializeField]
        private OpenMailDialog m_openMailDialog;

        [SerializeField]
        private SettingsWindow m_settingsWindow;
        
        [InlineEditor(Expanded = true)]
        [SerializeField]
        private UISpriteBank m_spriteBank;

        [InlineEditor(Expanded = true)]
        [SerializeField]
        private UIPrefabBank m_prefabBank;

        public Canvas TopmostCanvas => m_topmostCanvas;

        public AlertDialog AlertDialog => m_alertDialog;

        public WaitForRetryDialog WaitForRetryDialog => m_waitForRetryDialog;

        public ShopConfirmDialog ShopConfirmDialog => m_shopConfirmDialog;

        public UnlockHeroConfirmDialog UnlockHeroConfirmDialog => m_unlockHeroConfirmDialog;

        public ProgressDialog ProgressDialog => m_progressDialog;

        public WaitForServerResponse WaitForServerResponse => m_waitForServerResponse;

        public InspectItemWindow InspectItemWindow => m_inspectItemWindow;

        public CompareItemWindow CompareItemWindow => m_compareItemWindow;

        public OpenChestWindow OpenChestWindow => m_openChestWindow;

        public RectTransform CommonTransform => m_commonTransform;

        public UISpriteBank SpriteBank => m_spriteBank;
        
        public UIPrefabBank PrefabBank => m_prefabBank;

        public BlockerManager BlockerManager => m_blockerManager;

        public AccountWindow AccountWindow => m_accountWindow;

        public ShopWindow ShopWindow => m_shopWindow;

        public MailboxWindow MailboxWindow => m_mailboxWindow;

        public OpenMailDialog OpenMailDialog => m_openMailDialog;

        public SettingsWindow SettingsWindow => m_settingsWindow;

        protected override void Awake()
        {
            base.Awake();

            CanTick = true;
            RegisterService(this);
            Widget.SetBlockerSettings(CommonTransform, TopmostCanvas);
        }

        protected override void Start()
        {
            base.Start();

            var fpsCounter = FindObjectOfType<AFPSCounter>();
            var extender = fpsCounter.gameObject.AddComponent<AFPSCounterExtender>();
            extender.Mode = AFPSCounterExtenderMode.Fps;
        }

        private void HideAllTopLevelWidgets()
        {
            foreach (Widget widget in TopLevelWidgets)
            {
                widget.Hide();
            }
        }

        public void DisplayInGameMenu(bool showInstantly, InGameMode mode)
        {
            InGameMenuScreen.Show();
            InGameMenuScreen.Mode = mode;

            // if (showInstantly)
            // {
            //     // DeactivateAllTopLevelWidgets();
            //
            //     TopBar.Show();
            //     InGame.Show();
            //     InGame.Mode = mode;
            // }
            // else
            // {
            //     HideAllTopLevelWidgets();
            //
            //     TopBar.Show();
            //     InGame.Show();
            //     InGame.Mode = mode;
            // }
        }

        public void DisplayGameWin(int level)
        {
            GetWidget<InGameMenuScreen>().Hide();

            var gameWin = GetWidget<GameWin>();
            gameWin.SetLevel(level);
            gameWin.Show();
        }

        public void DisplayGameOverMenu()
        {
            // Hero animation, maybe

            GameOver.gameObject.SetActive(true);
        }

        public async UniTaskVoid DisplayMainMenu()
        {
            HideAllTopLevelWidgets();

            // First play time
            if (Game.IsFirstTime || Game.AuthenticationType == AuthenticationType.None)
            {
                await AccountWindow.ShowDialogAsync();
            }
            else if (Game.Player == null)
            {
                SignInReply signInReply = await Game.SignInWithSavedOptionAsync();

                if (signInReply != null)
                {
                    await Game.LoadPlayerDataAsync();
                }
                else
                {
                    await AccountWindow.ShowDialogAsync();
                }
            }

            Game.PrepareEmptyLevelForMainMenu();

            Player player = Game.Player;
            TopBar.Show();
            TopBar.SetPlayerDetails(player);

            await MainMenuScreen.ShowAsync(x => Debug.Log($"{x.name} is shown."));
            MainMenuScreen.TabBar.SetSelectedTab(2, true);
        }

        public override void Tick()
        {
            // if (Input.GetKeyDown(KeyCode.S))
            // {
            //     DisplayLoadout();
            // }
        }

        public void DisplayLoadout()
        {
        }

        public void ClearMainMenuResources()
        {
            var stopwatch = new Stopwatch(nameof(ClearMainMenuResources));

            Widget[] widgets = MainMenuScreen.GetComponentsInChildren<Widget>(true);

            foreach (Widget widget in widgets)
            {
                widget.OnResourcesUnloading();
            }

            stopwatch.Stop();
        }

        public void SwitchFPSMode()
        {
        }
    }
}
