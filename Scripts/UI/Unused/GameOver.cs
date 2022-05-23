using Armageddon.Games;
using Armageddon.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Unused
{
    public class GameOver : Widget
    {
        [SerializeField]
        private Button m_extraLifeButton;

        [SerializeField]
        private Button m_retryButton;

        // Start is called before the first frame update
        protected override void Start()
        {
            m_extraLifeButton.onClick.AddListener(OnExtraLifeClick);
            m_retryButton.onClick.AddListener(OnRetryButtonClick);
        }

        private void OnExtraLifeClick()
        {
            var game = GetService<Game>();
            game.ResurrectMotor();
        }

        private void OnRetryButtonClick()
        {
            // Game.ChangeState(OldGameState.OldMainMenu);
        }
    }
}
