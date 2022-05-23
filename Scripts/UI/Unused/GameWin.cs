using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Unused
{
    public class GameWin : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_levelClearedText;

        [SerializeField]
        private EventTrigger m_clickableAreaEventTrigger;

        protected override void Start()
        {
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(data => { OnScreenClick(); });
            m_clickableAreaEventTrigger.triggers.Add(entry);
        }

        public void SetLevel(int level)
        {
            m_levelClearedText.Set($"Level {level} is cleared!");
        }

        private void OnScreenClick()
        {
        }
    }
}
