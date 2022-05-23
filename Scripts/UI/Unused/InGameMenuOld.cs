using System;
using Armageddon.Games;
using Armageddon.Games.States;
using Armageddon.UI.Base;
using Armageddon.UI.InGameMenu;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.Unused
{
    public enum InGameModeOld
    {
        Normal,
        Demo
    }

    public class InGameMenuOld : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_livesText;

        [SerializeField]
        private Button m_pauseButton;

        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private GameObject m_pauseBackgroundObject;

        [SerializeField]
        private EventTrigger m_pauseBackgroundEventTrigger;

        [SerializeField]
        private GameObject m_buttonsObject;

        [SerializeField]
        private Button m_homeButton;

        [SerializeField]
        private Button m_playButton;

        [SerializeField]
        private Button m_settingsButton;

        private InGameMode m_mode;

        public InGameMode Mode
        {
            set => SetMode(value);
            get => m_mode;
        }

        protected override void Start()
        {
            m_pauseButton.onClick.AddListener(OnPauseButtonClicked);
            m_backButton.onClick.AddListener(OnBackButtonClicked);
            m_homeButton.onClick.AddListener(() => OnHomeButtonClicked().Forget());
            m_playButton.onClick.AddListener(OnPlayButtonClicked);
            m_settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            m_pauseBackgroundObject.SetActive(false);
            m_buttonsObject.SetActive(false);

            // Avoid assigning in editor as it's hard to refactor.
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(OnBackgroundObjectClick);
            m_pauseBackgroundEventTrigger.triggers.Add(entry);
        }

        private void SetMode(InGameMode mode)
        {
            m_mode = mode;

            if (m_mode == InGameMode.Normal)
            {
                m_pauseButton.gameObject.SetActive(true);
                m_backButton.gameObject.SetActive(false);
            }
            else if (mode == InGameMode.Demo)
            {
                m_pauseButton.gameObject.SetActive(false);
                m_backButton.gameObject.SetActive(true);
            }
        }

        private void OnBackButtonClicked()
        {
            var args = new MainMenuState { BackFromDemo = true };
            var game = GetService<Game>();

            // TODO: Re-implement
            // game.ChangeState(GameStateOld.MainMenu, args);
        }

        private async UniTaskVoid OnHomeButtonClicked()
        {
            var ui = GetService<UISystem>();

            bool? result = await ui.AlertDialog.ShowInfoDialogAsync("Attention",
                "Are you sure you want to quit to Main Menu?", "Yes", "Cancel");
            if (result == true)
            {
                var game = GetService<Game>();
                // // TODO: Re-implement
                // game.ChangeState(GameStateOld.MainMenu);
            }
        }

        private void OnPlayButtonClicked()
        {
            var game = GetService<Game>();
            game.Resume();
        }

        private void OnSettingsButtonClicked()
        {
        }

        private void OnPauseButtonClicked()
        {
            var game = GetService<Game>();
            game.Pause();
        }

        private void OnBackgroundObjectClick(BaseEventData arg0)
        {
            //Resume();
            //m_pauseBackgroundObject.SetActive(false);
        }

        public void SetLives(int lives)
        {
            m_livesText.text = $"LIVES: {lives}";
        }

        public void ToggleHeroStats()
        {
            // m_stats.ToggleState();
        }

        protected override void OnGamePaused(object sender, EventArgs e)
        {
            base.OnGamePaused(sender, e);

            m_pauseBackgroundObject.SetActive(true);
            m_pauseButton.gameObject.SetActive(false);
            m_buttonsObject.SetActive(true);
        }

        protected override void OnGameResumed(object sender, EventArgs e)
        {
            base.OnGameResumed(sender, e);

            m_pauseBackgroundObject.SetActive(false);
            m_pauseButton.gameObject.SetActive(true);
            m_buttonsObject.SetActive(false);
        }
    }
}
