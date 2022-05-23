using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Items
{
    public class ChestSheet : ItemSheet
    {
        [PreviewField(200)]
        [SerializeField]
        private Sprite m_image;

        public Sprite Image => m_image;
    }
}
