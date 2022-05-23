using Armageddon.UI.Base;
using UnityEngine;

namespace Armageddon.UI.MainMenu.Loadout.HeroSelection
{
    public class SelectHeroSubWindow : SubWindow
    {
        [SerializeField]
        private InspectHeroWindow m_inspectHeroWindow;

        [SerializeField]
        private HeroListWindow m_heroListWindow;

        protected override void OnEnable()
        {
            base.OnEnable();

            UI.TabPageBottomBar.BackButton.onClick.AddListener(OnBackButtonClicked);

            m_inspectHeroWindow.SetSelectedHero();
        }

        protected override void OnDisable()
        {
            UI.TabPageBottomBar.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            base.OnDisable();
        }

        // [SerializeField]
        // private Button m_backButton;

        protected override void OnInitialize()
        {
            m_inspectHeroWindow.Initialize();
            m_heroListWindow.Initialize();
        }

        private void OnBackButtonClicked()
        {
            Hide();
            SubWindowManager.SetSelectedSubWindow((int)LoadoutSubpageType.Inventory);
            UI.TabPageBottomBar.Hide();
            UI.MainMenuBar.Show();

            // m_selectHeroSubpage.Hide(false);
            // m_inventorySubpage.Show();
            //
            // var ui = GetService<UISystem>();
            // ui.MainMenuBar.Show();
            //
            // CurrentPanelType = LoadoutTabPagePanelType.Inventory;
            //
            // m_inspectHeroWindow.InspectingHero = null;
        }
    }
}
