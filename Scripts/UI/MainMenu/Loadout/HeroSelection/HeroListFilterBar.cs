using System;
using System.Linq;
using Armageddon.UI.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.HeroSelection
{
    public class HeroListFilterBar : Widget
    {
        public enum FilterType
        {
            All = 0,
            Unlocked = 1,

            Locked = 2
            // Favorite
        }

        [SerializeField]
        private Toggle[] m_toggles;

        [SerializeField]
        private HeroListWindow m_heroListWindow;

        [ShowInInspector]
        public FilterType CurrentFilter { private set; get; }

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (Toggle toggle in m_toggles)
            {
                toggle.isOn = false;

                string typeName = toggle.name.Replace("Toggle", string.Empty);
                var filterType = (FilterType)Enum.Parse(typeof(FilterType), typeName);
                toggle.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        OnFilterTypeChanged(filterType);
                    }
                    else
                    {
                        Toggle activeFilter = m_toggles.FirstOrDefault(x => x.isOn);

                        if (activeFilter == null)
                        {
                            OnFilterTypeChanged(FilterType.All);
                        }
                    }
                });
            }
        }

        protected override void OnDisable()
        {
            foreach (Toggle toggle in m_toggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }

            base.OnDisable();
        }

        [Button]
        private void RefreshToggles()
        {
            m_toggles = GetComponentsInChildren<Toggle>();

            foreach (Toggle toggle in m_toggles)
            {
                string typeName = toggle.name.Replace("Toggle", string.Empty);

                try
                {
                    var filterType = (FilterType)Enum.Parse(typeof(FilterType), typeName);
                    Debug.Log($"filterType = {filterType}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
        }

        public void FilterHeroes()
        {
            m_heroListWindow.ResetScrollBar();

            foreach (HeroElement heroToggle in m_heroListWindow.HeroElements)
            {
                switch (CurrentFilter)
                {
                    case FilterType.All:
                    {
                        heroToggle.gameObject.SetActive(true);
                        break;
                    }
                    case FilterType.Locked:
                    {
                        heroToggle.gameObject.SetActive(!heroToggle.Hero.IsUnlocked);

                        break;
                    }
                    case FilterType.Unlocked:
                    {
                        heroToggle.gameObject.SetActive(heroToggle.Hero.IsUnlocked);

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentFilter), CurrentFilter, null);
                }
            }
        }

        private void OnFilterTypeChanged(FilterType filterType)
        {
            CurrentFilter = filterType;
            FilterHeroes();
        }
    }
}
