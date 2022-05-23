using UnityEngine;

namespace Armageddon.Sheets
{
    public class MapSheet : ScriptableObject
    {
        [SerializeField]
        private string m_name;

        public string Name => m_name;
    }
}
