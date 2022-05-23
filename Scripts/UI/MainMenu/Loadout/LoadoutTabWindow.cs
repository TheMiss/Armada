using Armageddon.UI.Base;
using Armageddon.UI.MainMenu.Loadout.HeroSelection;
using Armageddon.UI.MainMenu.Loadout.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.MainMenu.Loadout
{
    public enum LoadoutSubpageType
    {
        Inventory,
        SelectHero
    }

    public class LoadoutTabWindow : TabWindow
    {
        [ChildGameObjectsOnly]
        [SerializeField]
        private SubWindowManager m_subWindowManager;
        
        [ChildGameObjectsOnly]
        [SerializeField]
        private InventorySubWindow m_inventorySubWindow;
        
        [ChildGameObjectsOnly]
        [SerializeField]
        private SelectHeroSubWindow m_selectHeroSubWindow;

        [SerializeField]
        private TabPageBottomBar m_bottomBar;
        
        public SubWindowManager SubWindowManager => m_subWindowManager;

        public InventorySubWindow InventorySubWindow => m_inventorySubWindow;

        public SelectHeroSubWindow SelectHeroSubWindow => m_selectHeroSubWindow;

        public TabPageBottomBar BottomBar => m_bottomBar;

        protected override void OnEnable()
        {
            base.OnEnable();

            SubWindowManager.SetSelectedSubWindow((int)LoadoutSubpageType.Inventory);
        }

        public override void ClearResources()
        {
        }
    }
}
