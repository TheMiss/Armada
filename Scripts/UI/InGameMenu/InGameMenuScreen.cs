using System;
using Armageddon.Backend.Functions;
using Armageddon.Games;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.InGameMenu
{
    public enum InGameMode
    {
        Normal,
        Demo
    }

    public class InGameMenuScreen : Widget
    {
        [SerializeField]
        private Button m_pauseButton;

        [SerializeField]
        private PauseWindow m_pauseWindow;

        [SerializeField]
        private BattleResultWindow m_battleResultWindow;

        private InGameMode m_mode;

        public InGameMode Mode
        {
            set => SetMode(value);
            get => m_mode;
        }

        public PauseWindow PauseWindow => m_pauseWindow;

        public BattleResultWindow BattleResultWindow => m_battleResultWindow;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        protected override void OnDisable()
        {
            m_pauseButton.onClick.RemoveAllListeners();
        }

        public override void Show(bool animate = true)
        {
            base.Show(animate);

            m_pauseButton.gameObject.SetActive(true);
            m_pauseWindow.gameObject.SetActive(false);
            m_battleResultWindow.gameObject.SetActive(false);
        }

        private void SetMode(InGameMode mode)
        {
            m_mode = mode;

            // if (m_mode == InGameMode.Normal)
            // {
            //     m_pauseButton.gameObject.SetActive(true);
            //     m_backButton.gameObject.SetActive(false);
            // }
            // else if (mode == InGameMode.Demo)
            // {
            //     m_pauseButton.gameObject.SetActive(false);
            //     m_backButton.gameObject.SetActive(true);
            // }
        }

        private void OnPauseButtonClicked()
        {
            var game = GetService<Game>();
            game.Pause();

            m_pauseWindow.Show();
        }

        protected override void OnGamePaused(object sender, EventArgs e)
        {
            base.OnGamePaused(sender, e);

            m_pauseButton.gameObject.SetActive(false);
        }

        protected override void OnGameResumed(object sender, EventArgs e)
        {
            base.OnGameResumed(sender, e);

            m_pauseButton.gameObject.SetActive(true);
        }

        public async UniTask<InGameResultWindowResult?> ShowBattleResultAsync(EndGameReply reply)
        {
            return await m_battleResultWindow.ShowResultAsync(reply);
        }
    }
}
