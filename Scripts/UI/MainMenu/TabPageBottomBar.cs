using System;
using Armageddon.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu
{
    [Flags]
    public enum TabPageBottomBarOptions
    {
        Back,
        Play
    }

    public class TabPageBottomBar : Widget
    {
        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private Button m_playButton;

        public Button BackButton => m_backButton;

        public Button PlayButton => m_playButton;

        public void ShowWithOptions(TabPageBottomBarOptions options = TabPageBottomBarOptions.Back)
        {
            BackButton.gameObject.SetActive(options.HasFlag(TabPageBottomBarOptions.Back));
            PlayButton.gameObject.SetActive(options.HasFlag(TabPageBottomBarOptions.Play));

            base.Show();
        }
    }
}
