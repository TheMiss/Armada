using System;
using Armageddon.Games;
using Armageddon.Games.States;
using Armageddon.Localization;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.InGameMenu
{
    public class PauseWindow : Window
    {
        [SerializeField]
        private GameObject m_pauseBackgroundObject;

        [SerializeField]
        private GameObject m_buttonsObject;

        [SerializeField]
        private Button m_homeButton;

        [SerializeField]
        private Button m_playButton;

        [SerializeField]
        private Button m_settingsButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_homeButton.onClick.AddListener(() => OnHomeButtonClickedAsync().Forget());
            m_playButton.onClick.AddListener(OnResumeButtonClicked);
            m_settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        protected override void OnDisable()
        {
            m_homeButton.onClick.RemoveAllListeners();
            m_playButton.onClick.RemoveAllListeners();
            m_settingsButton.onClick.RemoveAllListeners();
        }

        private void OnBackButtonClicked()
        {
            var args = new MainMenuState { BackFromDemo = true };
            var game = GetService<Game>();

            // TODO: Re-implement
            // game.ChangeState(GameStateOld.MainMenu, args);
        }

        private async UniTaskVoid OnHomeButtonClickedAsync()
        {
            string yes = Texts.UI.Yes;
            string cancel = Texts.UI.Cancel;
            string attention = Texts.UI.Attention;
            string askToExitBattle = Texts.UI.AskToExitBattle;
            bool? result = await UI.AlertDialog.ShowInfoDialogAsync(attention,
                askToExitBattle, yes, cancel);

            if (result == true)
            {
                Debug.Log("The battle is finished");
                UI.Game.World.EndGame();
            }
        }

        private void OnResumeButtonClicked()
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

        public void ToggleHeroStats()
        {
            // m_stats.ToggleState();
        }

        protected override void OnGamePaused(object sender, EventArgs e)
        {
            base.OnGamePaused(sender, e);

            m_pauseBackgroundObject.SetActive(true);
            m_buttonsObject.SetActive(true);
        }

        protected override void OnGameResumed(object sender, EventArgs e)
        {
            base.OnGameResumed(sender, e);

            m_pauseBackgroundObject.SetActive(false);
            m_buttonsObject.SetActive(false);
        }
    }
}
