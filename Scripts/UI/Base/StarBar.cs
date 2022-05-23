using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    public class StarBar : Widget
    {
        [SerializeField]
        private List<Toggle> m_starToggles;

        private int m_starCount;

        public int StarCount
        {
            get => m_starCount;
            set
            {
                m_starCount = value;
                SetActiveStars(value);
            }
        }

        private void SetActiveStars(int count)
        {
            foreach (Toggle starToggle in m_starToggles)
            {
                starToggle.gameObject.SetActive(false);
            }

            for (int i = 0; i < count; i++)
            {
                Toggle toggle = m_starToggles[i];
                toggle.gameObject.SetActive(true);
            }
        }
    }
}
