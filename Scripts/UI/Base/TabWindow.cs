using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.Base
{
    [DisallowMultipleComponent]
    public class TabWindow : Widget
    {
        [SerializeField]
        private Tab m_tab;

        

        [ReadOnly]
        public int Index => m_tab.Index;

        
        
        protected override void Awake()
        {
            base.Awake();
            
#if UNITY_EDITOR
            _SetTextMeshObjectNames();
#endif
        }

        public virtual void ClearResources()
        {
        }
    }
}
