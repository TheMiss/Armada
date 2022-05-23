using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.Base
{
    public class SubWindow : Widget
    {
        [SerializeField]
        private SubWindowManager m_subWindowManager;

        [ReadOnly]
        [SerializeField]
        private int m_index;

        private bool m_initialized;

        public int Index
        {
            get => m_index;
            set => m_index = value;
        }

        public SubWindowManager SubWindowManager
        {
            get => m_subWindowManager;
            set => m_subWindowManager = value;
        }
        
        protected virtual void OnInitialize()
        {
        }

        public override void Show(bool animate = true)
        {
            if (!m_initialized)
            {
                OnInitialize();

                m_initialized = true;
            }

            base.Show(animate);
        }
    }
}
