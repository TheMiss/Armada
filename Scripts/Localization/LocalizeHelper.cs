using System.Collections.Generic;
using I2.Loc;
using Purity.Common;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Armageddon.Localization
{
    public class LocalizeHelper : FastMonoBehaviour
    {
        [SerializeField]
        private RectTransform m_root;

        [SerializeField]
        private List<TextMeshProUGUI> m_noLocalizeComponentTexts = new();

        [Button]
        private void FindTextMeshWithoutLocalizeComponents()
        {
            m_noLocalizeComponentTexts.Clear();

            TextMeshProUGUI[] texts = m_root.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                var localize = text.GetComponent<Localize>();
                if (localize == null)
                {
                    m_noLocalizeComponentTexts.Add(text);
                }
            }
            
        }
    }
}
