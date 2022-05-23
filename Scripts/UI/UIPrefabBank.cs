using Armageddon.UI.Base;
using UnityEngine;

namespace Armageddon.UI
{
    public class UIPrefabBank : ScriptableObject
    {
        [SerializeField]
        private Badge m_badge;

        public Badge Badge => m_badge;
    }
}
