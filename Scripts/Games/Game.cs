using System;
using System.Collections.Generic;
using System.Threading;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Games.States;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Characters;
using Armageddon.UI;
using Armageddon.UI.Common;
using Armageddon.UI.MainMenu.PremiumShop;
using Armageddon.Worlds.Actors.Unused;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google;
using I2.Loc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using Purity.Common;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
#if ENABLE_FACEBOOK_SIGN_IN
using Facebook.Unity;
#endif

namespace Armageddon.Games
{
    public class Game : GameContext
    {
#if DEBUG
        [ShowInInspector]
        public bool CheckRequirementBeforeRequest { set; get; } = true;
#else
        public bool CheckRequirementBeforeRequest { set; get; } = true;
#endif

        [BoxGroupPrefabs]
        [SerializeField]
        private LocalizationSystem m_localizationSystem;

        [BoxGroupPrefabs]
        [SerializeField]
        private UISystem m_uiPrefab;

        [SerializeField]
        private Camera m_mainCamera;

        [SerializeField]
        private Background m_background;

        [SerializeField]
        private PlayerController m_playerController;

        [SerializeField]
        private FSMOwner m_fsmOwner;

        [SerializeField]
        private Blackboard m_blackboard;

        [SerializeField]
        private Transform m_minimapsTransform;

        public Player Player { get; set; }

        [ShowInPlayMode]
        public bool IsPausing { get; private set; }

        public PlayerController PlayerController => m_playerController;

        public BackendDriver BackendDriver { private set; get; }

        public Camera MainCamera => m_mainCamera;

        public Transform MinimapsTransform => m_minimapsTransform;

        private string FirstTimeKey => $"{m_appInstanceId}FirstTimeKey";
        private string LoginRememberKey => $"{m_appInstanceId}LoginRememberKey";
        private string RememberMeIdKey => $"{m_appInstanceId}RememberMeIdKey";
        private string RememberEmailKey => $"{m_appInstanceId}RememberedEmail";

        private string AuthenticationTypeKey => $"{m_appInstanceId}AuthenticationTypeKey";

        // Implement this to have different IDs so that it can be used for more than one instance.
        private readonly string m_appInstanceId = string.Empty;

        [ShowInPlayMode]
        public bool IsFirstTime
        {
            get => PlayerPrefs.GetInt(FirstTimeKey, 1) != 0;
            set => PlayerPrefs.SetInt(FirstTimeKey, value ? 1 : 0);
        }

        [ShowInPlayMode]
        public AuthenticationType AuthenticationType
        {
            get => (AuthenticationType)PlayerPrefs.GetInt(AuthenticationTypeKey, 0);
            private set => PlayerPrefs.SetInt(AuthenticationTypeKey, (int)value);
        }

        [ShowInPlayMode]
        public string RememberMeId
        {
            get => PlayerPrefs.GetString(RememberMeIdKey, "");
            set
            {
                string guid = value ?? Guid.NewGuid().ToString();
                PlayerPrefs.SetString(RememberMeIdKey, guid);
            }
        }

        [ShowInPlayMode]
        public string RememberEmail
        {
            get => PlayerPrefs.GetString(RememberEmailKey, "");
            set => PlayerPrefs.SetString(RememberEmailKey, value);
        }

        /// <summary>
        ///     Remember, Game is set to be the first too be initialized.
        ///     You can take a look at Script initialization order.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);
            Initialize();
        }

        private void Initialize()
        {
            // https://github.com/jilleJr/Newtonsoft.Json-for-Unity/issues/38
            AotHelper.EnsureType<StringEnumConverter>();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    }
                }
            };

            GameAccessibleObject.InjectGame(this);
            TagHandler.InjectGame(this);
#if DEBUG
            CheatCode.Create(this);
            // CheatCode.InjectGame(this);
            LocalizationManager.CurrentLanguage = "English";
#endif
            Debug.Log($"persistentDataPath: {Application.persistentDataPath}");

            DOTween.Init();

#if ENABLE_FACEBOOK_SIGN_IN
            SocialPlugins.InitializeFacebook();
#endif
#if ENABLE_GOOGLE_SIGN_IN
            SocialPlugins.InitializeGoogleSignIn();
#endif

            // BackendDriver = BackendDriver.Create(BackendDriverType.PlayFab);
            BackendDriver = BackendDriver.Create(BackendDriverType.PlayFab);
            BackendDriver.AuthenticationTypeChanged += OnAuthenticationTypeChanged;
            RegisterService(BackendDriver, typeof(BackendDriver));

            int siblingIndex = Transform.GetSiblingIndex();
            UISystem ui = Instantiate(m_uiPrefab);
            ui.RemoveCloneFromName();
            ui.Transform.SetSiblingIndex(siblingIndex + 1);
            // var mainCanvas = ui.GetComponent<Canvas>();

            LocalizationSystem localization = Instantiate(m_localizationSystem);
            localization.RemoveCloneFromName();
            localization.Transform.SetSiblingIndex(siblingIndex + 1);

            // var inAppPurchase = new InAppPurchase();
            
            UpdateLocalBadgesFileAsync().Forget();

            // GameState starts at Startup Main 
            // See the FSM of Game in Inspector
            
            CanTick = true;

            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor)
            {
            }
            else
            {
                Application.targetFrameRate = 60;
            }
        }

        private void OnAuthenticationTypeChanged(AuthenticationType authenticationType)
        {
            AuthenticationType previousType = AuthenticationType;
            AuthenticationType = authenticationType;

            // Emitted when signed out from BackendDriver
            if (AuthenticationType == AuthenticationType.None)
            {
                DeregisterService(Player);
                Player = null;
                RememberMeId = string.Empty;

                switch (previousType)
                {
                    case AuthenticationType.Facebook:
#if ENABLE_FACEBOOK_SIGN_IN
                        FB.LogOut();
#endif
                        break;
                    case AuthenticationType.Google:
#if ENABLE_GOOGLE_SIGN_IN
                        GoogleSignIn.DefaultInstance.SignOut();
#endif
                        break;
                }
            }
        }

        private void OnApplicationQuit()
        {
            Localization.SetLanguage(0);
            Game.Player?.WriteAllLocalSaveFiles();
            // Game.Player?.WritePlayerInventoryFile();
        }

        public override void Tick()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            bool inputHandled = false;

            if (Input.touchCount == 3)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    if (touch.position.y > Screen.height / 2.0f)
                    {
                    }

                    inputHandled = true;
                }
            }

            if (!inputHandled)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    inputHandled = true;
                    // UI.InGameMenuScreen.ToggleHeroStats();
                }
                else if (Input.GetMouseButtonDown(2))
                {
                    inputHandled = true;
                }
            }

            if (!inputHandled)
            {
            }
        }

        public void ExitApplication()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void PrepareEmptyLevelForMainMenu()
        {
            if (Player.CurrentHero == null)
            {
                Debug.LogWarning("Player.SelectedHero == null");
            }

            // CreateHeroActor(Player.CurrentHero);
            //CreatePlayableHero(Player.SelectedHero);
        }

        public void ClearObjects()
        {
        }

        public void ShowGameOverMenu()
        {
            UI.DisplayGameOverMenu();
            Pause();
        }

        public void ChangeBackground(int index)
        {
            m_background.Change(index);
        }

        public void ResurrectMotor()
        {
            UI.GameOver.gameObject.SetActive(false);
            // HeroActor.Damageable.SetStartHealth(1);
            Resume();
        }

        private void OnDamagerHit(object sender, DamagerHitArgs e)
        {
            // UI.InGame.SetLives(HeroActor.Damageable.CurrentHealth);
        }

        public void StartGame(StartGameReply reply)
        {
            Currency energy = Player.Currencies[CurrencyCode.Energy];
            int previousBalance = energy.Balance;

            energy.AddBalance(reply.Energy - previousBalance);

            var state = new BattleResourcesLoadingState
            {
                StartGameReply = reply
            };

            ChangeStateAsync(state).Forget();

            // var args = new LoadInGameResourcesState();
            // ChangeState(GameStateOld.LoadInGameResources, args);
        }

        public void StartDemo(Character character)
        {
            var args = new DemoResourcesLoadingState
            {
                Character = character
            };

            // TODO: Re-Implement
            // ChangeState(GameStateOld.LoadDemoResources, args);
        }

        public async UniTaskVoid ChangeStateAsync(GameState data)
        {
            await UI.ScreenFader.FadeOutAsync();

            m_fsmOwner.SendEvent(data.ToString(), data, this);
        }

        public void Pause()
        {
            if (IsPausing)
            {
                return;
            }

            ContextManager.Pause(this);
            // Should not do this as will also affect UI animation.
            // Time.timeScale = 0.0f;

            IsPausing = true;
        }

        public void Resume()
        {
            if (!IsPausing)
            {
                return;
            }

            ContextManager.Resume(this);
            // Time.timeScale = 1.0f;

            IsPausing = false;
        }

        public async UniTask<SignInReply> SignInWithSavedOptionAsync()
        {
            Game.UI.WaitForServerResponse.Show();
            Game.UI.TopBar.Hide();
            Game.UI.TabPageBottomBar.Hide();

            AuthenticationType authenticationType = Game.AuthenticationType;

            SignInReply reply = null;
            switch (authenticationType)
            {
                case AuthenticationType.Device:
                    reply = await Game.BackendDriver.SignInWithDeviceAsync();
                    break;
                case AuthenticationType.RememberMe:
                {
                    reply = await Game.BackendDriver.SignInWithRememberMeAsync(Game.RememberMeId);
                    UI.AccountWindow.SetEmail(reply.Email);
                }
                    break;
                case AuthenticationType.Facebook:
                {
#if ENABLE_FACEBOOK_SIGN_IN
                    // In case that FB is still initializing...
                    await UniTask.WaitWhile(() => !FB.IsInitialized);
                    reply = await UI.AccountWindow.SignInWithFacebookAsync(false);
#endif
                }
                    break;
                case AuthenticationType.Google:
                {
#if ENABLE_GOOGLE_SIGN_IN
                    reply = await UI.AccountWindow.SignInWithGoggleAsync(false);
#endif
                }
                    break;
            }

            if (reply != null)
            {
                DateTime? lastLoginTime = reply.LastLoginTime;

                if (lastLoginTime != null)
                {
                    Debug.Log($"LastLoginTime: {lastLoginTime.Value.ToStringEx()}");
                }
            }

            Game.UI.WaitForServerResponse.Hide();

            return reply;
        }

        public async UniTask<bool> LoadPlayerDataAsync()
        {
            Game.UI.WaitForServerResponse.Show();
            
            var tasks = new List<UniTask<bool>>
            {
                Game.BackendDriver.DownloadFileAsync(Player.PlayerProfileFileName),
                Game.BackendDriver.DownloadFileAsync(Player.PlayerInventoryFileName),
                Game.BackendDriver.DownloadFileAsync(Player.LocalBadgeManagerFileName)
            };

            while (true)
            {
                LoadPlayerReply reply = await Game.BackendDriver.LoadPlayerAsync(new LoadPlayerRequest());

                if (BackendReply.HasError(reply))
                {
                    // TODO: Localize messageText
                    string titleText = $"{Texts.UI.Error}";
                    string messageText = BackendReply.GetError(reply).FullMessage;
                    string acceptButtonText = Texts.UI.Retry;
                    await UI.AlertDialog.ShowErrorDialogAsync(titleText, messageText, acceptButtonText);

                    continue;
                }

                if (Game.Player == null)
                {
                    var player = new Player();
                    Game.Player = player;
                }

                // This doesn't work as expected!
                // await UniTask.WhenAll(tasks);
                
                // So it's either this or below.
                await tasks.WhenSucceeded();

                // if (tasks[0].Status != UniTaskStatus.Succeeded &&
                //     tasks[1].Status != UniTaskStatus.Succeeded &&
                //     tasks[2].Status != UniTaskStatus.Succeeded)
                // {
                //     Debug.Log($"tasks[0]:{tasks[0].Status}, tasks[1]:{tasks[1].Status}, tasks[2]:{tasks[2].Status}");
                //     await UniTask.Yield();
                // }

                await Game.Player.ReinitializeAsync(reply);

                break;
            }

            Game.UI.WaitForServerResponse.Hide();

            return true;
        }

        
        // public async UniTask<bool> LoadPlayerDataAsync()
        // {
        //     Game.UI.WaitForServerResponse.Show();
        //
        //     UniTask<bool> downloadTask1 = Game.BackendDriver.DownloadFileAsync(Player.PlayerProfileFileName);
        //     UniTask<bool> downloadTask2 = Game.BackendDriver.DownloadFileAsync(Player.PlayerInventoryFileName);
        //     // await Game.BackendDriver.DownloadFileAsync(Player.PlayerProfileFileName);
        //     // await Game.BackendDriver.DownloadFileAsync(Player.PlayerInventoryFileName);
        //
        //     while (true)
        //     {
        //         LoadPlayerReply reply = await Game.BackendDriver.LoadPlayerAsync(new LoadPlayerRequest());
        //
        //         if (BackendReply.HasError(reply))
        //         {
        //             // TODO: Localize
        //             string titleText = $"{Texts.UI.Error}";
        //             string messageText = BackendReply.GetError(reply).FullMessage;
        //             // messageText += "\n\nTap OK to retry.";
        //             string acceptButtonText = Texts.UI.Retry;
        //             await UI.AlertDialog.ShowErrorDialogAsync(titleText, messageText, acceptButtonText);
        //
        //             continue;
        //         }
        //
        //         if (Game.Player == null)
        //         {
        //             var player = new Player();
        //             Game.Player = player;
        //         }
        //
        //         if (downloadTask1.Status != UniTaskStatus.Succeeded &&
        //             downloadTask2.Status != UniTaskStatus.Succeeded)
        //         {
        //             Debug.Log($"Task1:{downloadTask1.Status}, Task2:{downloadTask2.Status}");
        //             await UniTask.Yield();
        //         }
        //
        //         await Game.Player.ReinitializeAsync(reply);
        //
        //         break;
        //     }
        //
        //     Game.UI.WaitForServerResponse.Hide();
        //
        //     return true;
        // }

        public void ReinitializePremiumShop()
        {
            var shopTabPage = FindObjectOfType<PremiumShopTabWindow>(true);
            shopTabPage.InitializePurchasing();
            shopTabPage.RefreshItems();
        }

        public bool ValidateReply(BackendReply reply)
        {
            if (!BackendReply.HasError(reply))
            {
                return true;
            }

            Error error = BackendReply.GetError(reply);

            switch (error.Type)
            {
#if DEBUG
                // You're cheating, aren't you?
                // Only available in DEBUG to let the game hanging in Loading state...
                case ErrorType.Fishy:
                {
                    string errorMessage = error.FullMessage;
                    Debug.LogWarning($"{errorMessage}");

                    // CheatDetected leads to MainMenu
                    m_blackboard.SetVariableValue("CheatDetected", true);

                    break;
                }
#endif
                case ErrorType.ApiError:
                {
                    if (error.CanWaitForRetry)
                    {
                        string fullMessage = error.FullMessage;
                        Debug.LogWarning($"{fullMessage} ({error.Code}) [{error.Type}]");

                        WaitForRetryDialog waitForRetryDialog = UI.WaitForRetryDialog;

                        waitForRetryDialog.SetDefaultTexts();
                        waitForRetryDialog.ShowDialogAsync(error.RetryDelay).Forget();
                    }
                    else
                    {
                        string fullMessage = error.FullMessage;
                        string fromParsing = error.CreatedErrorFromLogJsonObject ? " (from parsing)" : string.Empty;
                        Debug.LogWarning($"{error.Code}: {fullMessage}{fromParsing}");

                        string dialogMessage = error.DialogMessage;
                        string titleText = $"{Texts.UI.Error}!";
                        string acceptButtonText = Texts.UI.GotIt;
                        UI.AlertDialog.ShowErrorDialogAsync(titleText, dialogMessage, acceptButtonText).Forget();

                        // m_blackboard.SetVariableValue("GoToMainMenuRequested", true);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private async UniTask UpdateLocalBadgesFileAsync()
        {
            CancellationToken token = GetCancellationToken(nameof(UpdateLocalBadgesFileAsync));
            
            // We do polling every a second to see it there is any change since we don't use event-based approach.
            while (Application.isPlaying)
            {
                Player player = Game.Player;
                if (player != null)
                {
                    if (player.LocalBadgeManager.IsDataChanged)
                    {
                        player.WriteLocalBadgeManagerFile();
                        player.LocalBadgeManager.IsDataChanged = false;
                    }
                }

                await UniTask.Delay(1000, false, cancellationToken: token);
            }
        }
    }
}
