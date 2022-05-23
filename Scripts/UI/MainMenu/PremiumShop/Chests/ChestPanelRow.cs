using System.Collections.Generic;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop.Chests
{
    public class ChestPanelRow : Widget
    {
        [SerializeField]
        private GameObject m_topPanelObject;

        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_unlockNextChestText;

        [SerializeField]
        private TextMeshProUGUI m_unlockNextChestProgressText;

        [SerializeField]
        private Image m_unlockNextChestProgressFiller;

        [SerializeField]
        private ObjectHolderItem m_nextChestHolderItem;

        [SerializeField]
        private List<ChestPanelRowItemButton> m_buttons;

        public List<ChestPanelRowItemButton> Buttons => m_buttons;

        public void SetTitleText(string text)
        {
            m_titleText.Set(text);
        }

        public void SetUnlockNextChestProcess(int obtainedAmount, int requiredAmount)
        {
            string text = $"{obtainedAmount}/{requiredAmount}";
            m_unlockNextChestProgressText.Set(text);
            m_unlockNextChestProgressFiller.fillAmount = (float)obtainedAmount / requiredAmount;

            // We don't want to show a panel when the number has been reached.
            if (obtainedAmount >= requiredAmount || requiredAmount == 0)
            {
                m_topPanelObject.SetActive(false);
            }
        }
    }
}
