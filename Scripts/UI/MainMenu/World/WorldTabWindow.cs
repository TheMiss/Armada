using Armageddon.UI.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World
{
    public enum WorldTabSubpage
    {
        Main,
        SelectStage,
        SelectMap,
        InspectStage
    }

    public class WorldTabWindow : TabWindow
    {
        [ChildGameObjectsOnly]
        [SerializeField]
        private SubWindowManager m_subWindowManager;
        
        public SubWindowManager SubWindowManager => m_subWindowManager;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            UI.Game.MainCamera.orthographicSize = 10;

            SetSelectedSubpage(WorldTabSubpage.Main);
        }

        protected override void OnDisable()
        {
            // This mostly occurs when stop playing as MainCamera would be destroyed before this component.
            if (UI.Game.MainCamera != null)
            {
                UI.Game.MainCamera.orthographicSize = 20;
            }

            base.OnDisable();
        }

        public void SetSelectedSubpage(WorldTabSubpage subpage, bool animate = true)
        {
            SubWindowManager.SetSelectedSubWindow((int)subpage, animate);
        }
    }
}
