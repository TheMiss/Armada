using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Games;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.MapSelection
{
    public class SelectMapSubWindow : SubWindow
    {
        [BoxGroupPrefabs]
        [AssetsOnly]
        [SerializeField]
        private MinimapWindow m_minimapWindowPrefab;

        [BoxGroupPrefabs]
        [AssetsOnly]
        [SerializeField]
        private Camera m_cameraPrefab;

        [SerializeField]
        private SimpleScrollSnap m_scrollSnap;

        [SerializeField]
        private RectTransform m_contentTransform;

        [SerializeField]
        private TextMeshProUGUI m_mapTitleText;

        [SerializeField]
        private Button m_selectMapButton;

        [HideInEditorMode]
        [ShowInInspector]
        public List<MinimapWindow> MinimapWindows { set; get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Needs to destroy immediately as m_scrollSnap will initialize things at awake and set up stuff at Start().
            // If children are not destroyed immediately, SimpleScrollSnap will gather in-queue-to-destroy panels
            // and then would soon access already destroyed objects.
            // Otherwise, we should CreateMinimaps() after SimpleScrollSnap is ready (after SimpleScrollSnap's Start() is called).
            m_contentTransform.DestroyDesignRemnant(true);

            if (MinimapWindows == null)
            {
                CreateMinimaps();
            }

            m_scrollSnap.onPanelSelected.AddListener(OnPanelSelected);
            m_scrollSnap.onPanelChanged.AddListener(OnPanelChanged);

            m_selectMapButton.onClick.AddListener(OnSelectMapButtonClicked);

            UI.Game.MinimapsTransform.gameObject.SetActive(true);
            UI.MainMenuBar.Hide();

            UI.TabPageBottomBar.ShowWithOptions();
            UI.TabPageBottomBar.BackButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnDisable()
        {
            m_scrollSnap.onPanelSelected.RemoveListener(OnPanelSelected);
            m_scrollSnap.onPanelChanged.RemoveListener(OnPanelChanged);

            m_selectMapButton.onClick.RemoveListener(OnSelectMapButtonClicked);

            UI.Game.MinimapsTransform.gameObject.SetActive(false);
            UI.MainMenuBar.Show();

            UI.TabPageBottomBar.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            base.OnDisable();
        }

        private void OnBackButtonClicked()
        {
            SubWindowManager.SetSelectedSubWindow((int)WorldTabSubpage.SelectStage);
            // WorldTabWindow.SetSelectedSubpage(WorldTabSubpage.SelectStage);
        }

        private void OnPanelSelected()
        {
            // Debug.Log("OnPanelSelected");
        }

        private void OnPanelChanged()
        {
            // Debug.Log("OnPanelChanged");
            int mapId = m_scrollSnap.CurrentPanel;
            m_mapTitleText.Set(Lexicon.MapName(mapId));

            AdjustPreviousAndNextButtons();
        }

        private void OnSelectMapButtonClicked()
        {
            int mapId = m_scrollSnap.CurrentPanel;
            Player player = UI.Game.Player;
            player.CurrentMapId = mapId;

            SubWindowManager.SetSelectedSubWindow((int)WorldTabSubpage.SelectStage);
            // WorldTabWindow.SetSelectedSubpage(WorldTabSubpage.SelectStage);
        }

        private void AdjustPreviousAndNextButtons()
        {
            int index = m_scrollSnap.CurrentPanel;

            if (m_scrollSnap.Panels.Length > 1)
            {
                if (index == 0)
                {
                    m_scrollSnap.previousButton.gameObject.SetActive(false);
                    m_scrollSnap.nextButton.gameObject.SetActive(true);
                }
                else if (index == m_scrollSnap.Panels.Length - 1)
                {
                    m_scrollSnap.previousButton.gameObject.SetActive(true);
                    m_scrollSnap.nextButton.gameObject.SetActive(false);
                }
                else
                {
                    m_scrollSnap.previousButton.gameObject.SetActive(true);
                    m_scrollSnap.nextButton.gameObject.SetActive(true);
                }
            }
            else
            {
                m_scrollSnap.previousButton.gameObject.SetActive(false);
                m_scrollSnap.nextButton.gameObject.SetActive(false);
            }
        }

        [SuppressMessage("ReSharper", "LocalVariableHidesMember")]
        private void CreateMinimaps()
        {
            var game = GetService<Game>();

            MinimapWindows = new List<MinimapWindow>();

            int maxId = 3;

            for (int i = 1; i <= maxId; i++)
            {
                Camera camera = Instantiate(m_cameraPrefab, game.MinimapsTransform);
                camera.name = $"MinimapCamera{i}";
                camera.transform.position = new Vector3(24.75f * i, 0, -100);

                var renderTexture = new RenderTexture(1024, 1024, 24);
                camera.targetTexture = renderTexture;

                MinimapWindow minimapWindow = Instantiate(m_minimapWindowPrefab, m_contentTransform);
                minimapWindow.LoadAsync(i, camera.transform).Forget();
                minimapWindow.MinimapRenderTexture.texture = renderTexture;
                MinimapWindows.Add(minimapWindow);
            }

            // foreach (MinimapWindow minimapWindow in MinimapWindows)
            // {
            //     m_scrollSnap.Add(minimapWindow.gameObject, minimapWindow.MapId);
            // }
        }
    }
}
