using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Armageddon.UI.Base
{
    public class SubWindowManager : Widget
    {
        [FormerlySerializedAs("m_subpages")]
        [ReadOnly]
        [SerializeField]
        private List<SubWindow> m_subWindows;

        public SubWindow SelectedSubWindow { private set; get; }

        protected override void Awake()
        {
            base.Awake();
            
            CollectSubWindows();
        }
        
        [DefaultButton]
        private void CollectSubWindows()
        {
            m_subWindows = new List<SubWindow>();

            int index = 0;
            foreach (Transform childTransform in Transform)
            {
                var subpage = childTransform.GetComponent<SubWindow>();

                if (subpage != null)
                {
                    subpage.Index = index++;
                    subpage.SubWindowManager = this;
                    m_subWindows.Add(subpage);
                }
            }
        }

        public void SetSelectedSubWindow(int index, bool animate = true)
        {
            // if (SelectedSubPage != null)
            // {
            //     if (SelectedSubPage.Index == index)
            //     {
            //         return;
            //     }
            // }

            SubWindow previousSelectedPage = SelectedSubWindow;

            List<SubWindow> subWindows = m_subWindows.Where(x => x.Index == index).ToList();

            foreach (SubWindow subWindow in subWindows)
            {
                if (SelectedSubWindow != null)
                {
                    SelectedSubWindow.Hide(false);
                }

                SelectedSubWindow = subWindow;
                SelectedSubWindow.Show(animate);
            }

            if (subWindows.Count == 0)
            {
                Debug.LogWarning($"Could not find index{index}");
            }
        }

        public T GetSubWindow<T>() where T : SubWindow
        {
            return m_subWindows.Where(x => x.GetType() == typeof(T)).Cast<T>().FirstOrDefault();
        }
    }
}
