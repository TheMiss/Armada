using Purity.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Armageddon.Sheets
{
    public abstract class Sheet : ScriptableObject, IIdentifiable
    {
        [HideInInspector]
        [SerializeField]
        private int m_id;

        [TableColumnWidth(56, false)]
        [PreviewField(ObjectFieldAlignment.Left)]
        [PropertyOrder(-10)]
        [SerializeField]
        private Sprite m_icon;

        [PropertyOrder(-10)]
        [DisplayAsString]
        [SerializeField]
        private string m_name;

        public string Name => m_name;

        public Sprite Icon => m_icon;

        public int Id => m_id;

        public override string ToString()
        {
            return Name;
        }

#if UNITY_EDITOR

        public void _SetId(int id)
        {
            m_id = id;
            EditorUtility.SetDirty(this);
        }

        public void _SetName(string name)
        {
            m_name = name;
            EditorUtility.SetDirty(this);
        }

        public void _SetIcon(Sprite icon)
        {
            m_icon = icon;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
