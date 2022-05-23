using System;
using Armageddon.AssetManagement;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Games;
using Armageddon.Localization;
using Armageddon.Maps;
using Armageddon.UI.Base;
using Armageddon.UI.MainMenu.World.StartGame;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.StageSelection
{
    public class SelectStageSubWindow : SubWindow
    {
        [Required]
        [SerializeField]
        private GameObject m_unmaskBackgroundObject;

        [Required]
        [SerializeField]
        private InspectStageWindow m_inspectStageWindow;

        [Required]
        [SerializeField]
        private Widget m_switchMapPanel;

        [Required]
        [SerializeField]
        private TextMeshProUGUI m_mapTitleText;

        [Required]
        [SerializeField]
        private Button m_switchMapButton;

        [HideInEditorMode]
        [ShowInInspector]
        public MapNode MapNode { set; get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_switchMapPanel.gameObject.SetActive(false);

            int mapId = UI.Game.Player.CurrentMapId;
            LoadMapAsync(mapId).Forget();
            ShowSwitchMapPanelAsync().Forget();

            m_switchMapButton.onClick.AddListener(OnSwitchMapButtonClicked);

            m_unmaskBackgroundObject.SetActive(true);

            UI.MainMenuScreen.SetBackgroundEnabled(false);
            UI.MainMenuBar.Hide();

            UI.TabPageBottomBar.ShowWithOptions();
            UI.TabPageBottomBar.BackButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnDisable()
        {
            if (MapNode != null)
            {
                //Map.StageButtonClicked.RemoveAllListeners();
                MapNode.gameObject.SetActive(false);
            }

            m_switchMapButton.onClick.RemoveListener(OnSwitchMapButtonClicked);

            m_unmaskBackgroundObject.SetActive(false);
            UI.MainMenuScreen.SetBackgroundEnabled(true);
            UI.MainMenuBar.Show();

            UI.TabPageBottomBar.Hide();
            UI.TabPageBottomBar.BackButton.onClick.RemoveAllListeners();
            // UI.TabPageBottomBar.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            base.OnDisable();
        }

        private void OnBackButtonClicked()
        {
            SubWindowManager.SetSelectedSubWindow((int)WorldTabSubpage.Main);
            // WorldTabWindow.SetSelectedSubpage(WorldTabSubpage.Main);
        }

        public override void OnResourcesUnloading()
        {
            DestroyGameObject(MapNode);
            MapNode = null;
        }

        private async UniTaskVoid ShowSwitchMapPanelAsync()
        {
            // Simple workaround without animation finish callback, TabOpenController will finish in 0.2 seconds.
            await UniTask.Delay(150);

            m_switchMapPanel.Show();
        }

        private async UniTaskVoid LoadMapAsync(int mapId)
        {
            if (MapNode != null && MapNode.MapId == mapId)
            {
                MapNode.gameObject.SetActive(true);
                return;
            }

            DestroyGameObject(MapNode);

            MapNode = await Assets.InstantiateMapNodeAsync(mapId);
            MapNode.Transform.SetSiblingIndex(UI.Transform.GetSiblingIndex());
            MapNode.StageButtonClicked.AddListener(OnStageNodeButtonClicked);

            m_mapTitleText.Set(Lexicon.MapName(mapId));
        }

        private void OnStageNodeButtonClicked(StageNode stageNode)
        {
            Debug.Log($"Clicked on Stage {stageNode.StageId}");
            InspectStageAsync(stageNode).Forget();

            // var inspectStageSubpage = WorldTabPage.SubpageManager.GetSubpage<InspectStageWindow>();
            // inspectStageSubpage.Inspect(stageNode);
            //
            // var tilemapMover = stageNode.MapNode.GetComponent<TilemapMover>();
            // tilemapMover.CanDrag = false;
            // UI.TabPageBottomBar.Show(TabPageBottomBarOptions.Back | TabPageBottomBarOptions.Play);
            // UI.MainMenuBar.Hide();
        }

        private async UniTaskVoid InspectStageAsync(StageNode stageNode)
        {
            TilemapMover tilemapMover = stageNode.MapNode.TilemapMover;

            tilemapMover.CanDrag = false;

            // Remove the listener so that we don't get the back button clicked event when inspecting stage
            UI.TabPageBottomBar.BackButton.onClick.RemoveListener(OnBackButtonClicked);
            UI.TabPageBottomBar.ShowWithOptions(TabPageBottomBarOptions.Back | TabPageBottomBarOptions.Play);
            // UI.MainMenuBar.Hide();

            InspectStageWindowResult? result = await m_inspectStageWindow.InspectAsync(stageNode);

            switch (result)
            {
                case InspectStageWindowResult.Back:
                    // UI.TabPageBottomBar.Hide();
                    // UI.MainMenuBar.Show();
                    Debug.Log("Let's Go Back!");
                    break;
                case InspectStageWindowResult.Play:
                    Debug.Log("Let's Play!");
                    StartGameAsync(stageNode).Forget();
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UI.TabPageBottomBar.ShowWithOptions();
            tilemapMover.CanDrag = true;
            // Add the listener back after done inspecting.
            UI.TabPageBottomBar.BackButton.onClick.AddListener(OnBackButtonClicked);
            // UI.TabPageBottomBar.Hide();
            // UI.MainMenuBar.Show();
        }

        private async UniTaskVoid StartGameAsync(StageNode stageNode)
        {
            UI.WaitForServerResponse.Show();

            var game = GetService<Game>();
            var backendDriver = GetService<BackendDriver>();

            var request = new StartGameRequest
            {
                MapId = stageNode.MapId,
                StageId = stageNode.StageId
            };

            StartGameReply startGameReply = await backendDriver.StartGameAsync(request);

            UI.WaitForServerResponse.Hide();

            if (!game.ValidateReply(startGameReply))
            {
                return;
            }

            UI.MainMenuScreen.Hide();
            UI.TopBar.Hide();

            await UI.ScreenFader.FadeOutAsync();

            Debug.Log("Ready to Load Stage!");
            game.StartGame(startGameReply);
        }

        private void OnSwitchMapButtonClicked()
        {
            SubWindowManager.SetSelectedSubWindow((int)WorldTabSubpage.SelectMap);
            //WorldTabWindow.SetSelectedSubpage(WorldTabSubpage.SelectMap);

            // UI.TabPageBottomBar.Show();
            // UI.MainMenuBar.Hide();
        }
    }
}
