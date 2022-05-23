using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.Main
{
    public class MainSubWindow : SubWindow
    {
        [SerializeField]
        private Button m_shopButton;

        [SerializeField]
        private Button m_hamburgerButton;

        [SerializeField]
        private Button m_playButton;

        [SerializeField]
        private MoreMenuWindow m_moreMenuWindow;

        protected override void Awake()
        {
            base.Awake();

            m_shopButton.onClick.AddListener(OnShopButtonClicked);
            m_hamburgerButton.onClick.AddListener(OnHamburgerButtonClicked);
            m_playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnShopButtonClicked()
        {
            UI.ShopWindow.Show();
            UI.TopBar.Transform.SetAsLastSibling();
        }

        private void OnHamburgerButtonClicked()
        {
            m_moreMenuWindow.BlockerColor = UI.BlockerManager.ClearColor;
            m_moreMenuWindow.ShowDialogAsync(true, true, true).Forget();
        }

        private void OnPlayButtonClicked()
        {
            SubWindowManager.SetSelectedSubWindow((int)WorldTabSubpage.SelectStage);
            // WorldTabWindow.SetSelectedSubpage(WorldTabSubpage.SelectStage);
        }
    }
}
