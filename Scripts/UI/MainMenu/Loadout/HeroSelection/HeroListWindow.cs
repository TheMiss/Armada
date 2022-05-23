using System.Collections.Generic;
using System.Linq;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Characters;
using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.HeroSelection
{
    public class HeroListWindow : Window
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private HeroElement m_heroElementPrefab;

        [SerializeField]
        private TextMeshProUGUI m_unlockedNumberText;

        [SerializeField]
        private TextMeshProUGUI m_totalNumberText;

        [SerializeField]
        private ToggleGroup m_toggleGroup;

        [SerializeField]
        private RectTransform m_contentTransform;

        [SerializeField]
        private InspectHeroWindow m_inspectHeroWindow;

        [SerializeField]
        private HeroListFilterBar m_filterBar;

        private List<HeroElement> m_heroElements;

        public List<HeroElement> HeroElements => m_heroElements;

        public HeroListFilterBar FilterBar => m_filterBar;

        public void Initialize()
        {
            m_contentTransform.DestroyDesignRemnant();

            var player = GetService<Player>();
            m_heroElements = new List<HeroElement>();

            foreach (Hero hero in player.Heroes)
            {
                // One time create, no need to deregister events. So no double events.
                HeroElement heroElement = Instantiate(m_heroElementPrefab, m_toggleGroup.transform, false);
                heroElement.SetIcon(hero.Sheet.Icon);
                heroElement.SetTitle($"{hero.Sheet.Name}");
                heroElement.Hero = hero;
                heroElement.Toggle.group = m_toggleGroup;
                heroElement.Toggle.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        m_inspectHeroWindow.OnHeroElementSelected(hero);
                    }
                });

                m_heroElements.Add(heroElement);
            }

            RefreshUnlockedNumber();
        }

        public void RefreshUnlockedNumber()
        {
            var player = GetService<Player>();
            int unlockedCount = player.Heroes.Count(x => x.IsUnlocked);
            int allCount = player.Heroes.Count;

            m_unlockedNumberText.Set($"{unlockedCount}");
            m_totalNumberText.Set($"{allCount}");
        }

        public void SetSelectedHero(Hero hero)
        {
            foreach (HeroElement heroElement in m_heroElements)
            {
                if (heroElement.Hero == hero)
                {
                    heroElement.Select();
                }
                else
                {
                    heroElement.Deselect();
                }
            }
        }

        public void ResetScrollBar()
        {
            Vector2 anchoredPosition = m_contentTransform.anchoredPosition;
            anchoredPosition.y = 0.0f;
            m_contentTransform.anchoredPosition = anchoredPosition;
        }
    }
}
