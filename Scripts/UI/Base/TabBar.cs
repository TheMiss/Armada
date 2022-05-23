using System.Collections.Generic;
using System.Linq;
using Armageddon.Configuration;
using Armageddon.Externals.OdinInspector;
using DG.Tweening;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Base
{
    public class TabBar : Widget, IPointerDownHandler
    {
        [SerializeField]
        private List<Tab> m_tabs = new();
        
        [SerializeField]
        private RectTransform m_selectedTabHighlighter;
        
        [SerializeField]
        private float m_selectionMoveDuration = 0.2f;
        
        [ReadOnly]
        [SerializeField]
        private Tab m_selectedTab;

        [SerializeField]
        private float m_selectedIconScale = 1.2f;

        [SerializeField]
        private float m_selectedTextScale = 1.2f;

        [SerializeField]
        private float m_tweenDuration = 0.2f;

        private bool m_highlighterIsPlaying;

        public TabChangedEvent TabChanged { get; } = new();

        public Tab SelectedTab
        {
            private set => m_selectedTab = value;
            get => m_selectedTab;
        }

        public TabWindow CurrentTabWindow => SelectedTab.TabWindow;

        protected override void Awake()
        {
            base.Awake();

            m_tabs = GetComponentsInChildren<Tab>(true).ToList();

            for (int i = 0; i < m_tabs.Count; i++)
            {
                m_tabs[i].Index = i;
            }

            // Bad idea to call this in Awake() as it causes SetSelectedTab to be called twice, hence double register events
            //SetSelectedTab(0);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_highlighterIsPlaying)
            {
                // return;
            }

            var tab = TestHit<Tab>(eventData);

            if (tab == null)
            {
                return;
            }

            if (SelectedTab == tab)
            {
                return;
            }
            
            SetSelectedTab(tab.Index);
            //Debug.Log($"tab = {tab.name}.");
        }

        public void SetSelectedTab(int index, bool snapImmediately = false)
        {
            Tab previousSelectedTab = SelectedTab;
            foreach (Tab tab in m_tabs.Where(tab => tab.Index == index))
            {
                if (SelectedTab != null)
                {
                    SelectedTab.TabWindow.Hide(false);
                    SelectedTab.Deselect(m_tweenDuration);
                }

                SelectedTab = tab;
                SelectedTab.Select(m_selectedIconScale, m_selectedTextScale, m_tweenDuration);
                SelectedTab.TabWindow.Show();

                m_highlighterIsPlaying = true;

                m_selectedTabHighlighter.DOMove(SelectedTab.Transform.position, m_selectionMoveDuration).OnComplete(
                    () =>
                    {
                        TabChanged.Invoke(index);
                        m_highlighterIsPlaying = false;

                        if (DebugSettings.ShowMoveSelection)
                        {
                            Debug.Log("Selection move finished");
                        }
                    });


                if (snapImmediately)
                {
                    var rectTransform = m_selectedTabHighlighter.GetComponent<RectTransform>();
                    rectTransform.transform.position = SelectedTab.Transform.position;
                }

                if (!Application.isPlaying)
                {
                    m_selectedTabHighlighter.position = SelectedTab.Transform.position;
                    previousSelectedTab.TabWindow.gameObject.SetActive(false);
                    SelectedTab.TabWindow.gameObject.SetActive(true);
                }
            }

            // SelectedTab.Show();
        }
        
        [DefaultButton]
        private void CollectTab()
        {
            m_tabs = GetComponentsInChildren<Tab>(true).ToList();

            for (int i = 0; i < m_tabs.Count; i++)
            {
                m_tabs[i].Index = i;
            }

            TabWindow[] tabPages = GetComponentsInChildren<TabWindow>(true);

            foreach (TabWindow panel in tabPages)
            {
                Tab tab = m_tabs.FirstOrDefault(x => panel.name.Contains(x.name));
                if (tab != null)
                {
                    ReflectionUtility.SetField(tab, "m_page", panel);
                    ReflectionUtility.SetField(panel, "m_tab", tab);
                }
            }

            if (m_selectedTab == null)
            {
                SetSelectedTab(0);
            }
        }

        [DefaultButton]
        // [GUIColorDefaultButton]
        private void SetPreviousTab()
        {
            int index = SelectedTab.Index;
            index--;
            if (index < 0)
            {
                index = m_tabs.Count - 1;
            }

            SetSelectedTab(index);
        }

        [DefaultButton]
        // [GUIColorDefaultButton]
        private void SetNextTab()
        {
            int index = SelectedTab.Index;
            index++;
            if (index > m_tabs.Count - 1)
            {
                index = 0;
            }

            SetSelectedTab(index);
        }

        public void ClearResources()
        {
            foreach (Tab tab in m_tabs)
            {
                tab.TabWindow.ClearResources();
            }
        }

        public class TabChangedEvent : UnityEvent<int>
        {
        }
    }
}
