using System;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Games;
using Armageddon.UI.Base;
using Armageddon.UI.Common;
using Armageddon.UI.InGameMenu;
using Armageddon.UI.MainMenu;
using Armageddon.UI.Unused;
using Armageddon.Worlds;
using Purity.Common;
using UnityEngine;

namespace Armageddon.UI
{
    /// <summary>
    ///     Intentionally used for UISystem to separate auto locate service properties.
    /// </summary>
    public abstract class UISystemBase : Context
    {
        private static Game m_game;

        private readonly List<Widget> m_topLevelWidgets = new();
        private readonly List<Widget> m_widgets = new();

        private GameOver m_gameOver;

        private InGameMenuScreen m_inGameMenuScreen;

        private MainMenuScreen m_mainMenuScreen;

        private MainMenuBar m_mainMenuBar;

        private ScreenFader m_screenFader;

        private TabPageBottomBar m_tabPageBottomBar;

        private TopBar m_topbar;

        private PreviewManager m_previewManager;

        private StatsMonitorBar m_statsMonitorBar;

        public Game Game
        {
            get
            {
                if (m_game == null)
                {
                    m_game = GetService<Game>();
                }

                return m_game;
            }
        }

        public TopBar TopBar
        {
            get
            {
                if (m_topbar == null)
                {
                    m_topbar = GetWidget<TopBar>();
                }

                return m_topbar;
            }
        }

        public MainMenuScreen MainMenuScreen
        {
            get
            {
                if (m_mainMenuScreen == null)
                {
                    m_mainMenuScreen = GetWidget<MainMenuScreen>();
                }

                return m_mainMenuScreen;
            }
        }

        public MainMenuBar MainMenuBar
        {
            get
            {
                if (m_mainMenuBar == null)
                {
                    m_mainMenuBar = GetWidget<MainMenuBar>();
                }

                return m_mainMenuBar;
            }
        }

        public TabPageBottomBar TabPageBottomBar
        {
            get
            {
                if (m_tabPageBottomBar == null)
                {
                    m_tabPageBottomBar = GetWidget<TabPageBottomBar>();
                }

                return m_tabPageBottomBar;
            }
        }

        public GameOver GameOver
        {
            get
            {
                if (m_gameOver == null)
                {
                    m_gameOver = GetWidget<GameOver>();
                }

                return m_gameOver;
            }
        }

        public InGameMenuScreen InGameMenuScreen
        {
            get
            {
                if (m_inGameMenuScreen == null)
                {
                    m_inGameMenuScreen = GetWidget<InGameMenuScreen>();
                }

                return m_inGameMenuScreen;
            }
        }

        public ScreenFader ScreenFader
        {
            get
            {
                if (m_screenFader == null)
                {
                    m_screenFader = GetWidget<ScreenFader>();
                }

                return m_screenFader;
            }
        }

        public PreviewManager PreviewManager
        {
            get
            {
                if (m_previewManager == null)
                {
                    m_previewManager = GetService<PreviewManager>();
                }

                return m_previewManager;
            }
        }

        public StatsMonitorBar StatsMonitorBar
        {
            get
            {
                if (m_statsMonitorBar == null)
                {
                    m_statsMonitorBar = GetWidget<StatsMonitorBar>();
                }

                return m_statsMonitorBar;
            }
        }

        protected IReadOnlyList<Widget> Widgets => m_widgets;

        protected IReadOnlyList<Widget> TopLevelWidgets => m_topLevelWidgets;


        protected override void Awake()
        {
            base.Awake();

            Widget[] widgets = GetComponentsInChildren<Widget>(true);
            m_widgets.AddRange(widgets);

            foreach (Transform child in Transform)
            {
                var widget = child.GetComponent<Widget>();

                if (widget != null)
                {
                    m_topLevelWidgets.Add(widget);
                }
            }
        }

        public T GetWidget<T>() where T : Widget
        {
            Type type = typeof(T);
            var control = m_widgets.FirstOrDefault(x => x.GetType() == type) as T;

            if (control == null)
            {
                Debug.LogError($"{name}: Could not get {type}!");
            }

            return control;
        }
    }
}
