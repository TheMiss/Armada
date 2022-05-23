using System.Collections.Generic;
using System.Linq;
using Armageddon.UI.Base;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Upgrades
{
    public enum UpgradesTabSubpage
    {
        Abilities,
        Cards
    }

    public class UpgradesTabWindow : TabWindow
    {
        [ChildGameObjectsOnly]
        [SerializeField]
        private SubWindowManager m_subWindowManager;
        
        [SerializeField]
        private GameObject m_subpageTogglesObject;

        [SerializeField]
        private Toggle m_abilitiesToggle;

        [SerializeField]
        private Toggle m_cardsToggle;
        
        [ShowInPlayMode]
        private List<Toggle> m_toggles;

        private Toggle m_selectedToggle;
        
        public SubWindowManager SubWindowManager => m_subWindowManager;

        protected override void Awake()
        {
            base.Awake();
            
            m_toggles = m_subpageTogglesObject.GetComponentsInChildren<Toggle>().ToList();

            foreach (Toggle toggle in m_toggles)
            {
                // toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        OnToggleChanged(toggle);
                    }
                });
            }
            
            SetSelectedPage(UpgradesTabSubpage.Abilities);
            m_subpageTogglesObject.SetActive(true);

        }

        private void OnToggleChanged(Toggle toggle)
        {
            if (m_selectedToggle == toggle)
            {
                return;
            }

            if (toggle == m_abilitiesToggle)
            {
                Debug.Log("This is m_abilitiesToggle.");
                SetSelectedPage(UpgradesTabSubpage.Abilities);
            }
            else if (toggle == m_cardsToggle)
            {
                Debug.Log("This is m_cardsToggle.");
                SetSelectedPage(UpgradesTabSubpage.Cards);
            }

            m_selectedToggle = toggle;
        }

        public void SetSelectedPage(UpgradesTabSubpage subpage, bool animate = true)
        {
            SubWindowManager.SetSelectedSubWindow((int)subpage, animate);
        }
    }
}
