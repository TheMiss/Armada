using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.Sheets.Items;
using Armageddon.UI.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Armageddon.UI.MainMenu.PremiumShop.Chests
{
    public class ChestsPanel : Panel
    {
        [SerializeField]
        private List<ChestPanelRow> m_rows;

        public void AddButtonListener(UnityAction<PremiumShopItemButton> callback)
        {
            foreach (ChestPanelRow row in m_rows)
            {
                row.Buttons[0].Button.onClick.AddListener(() => callback(row.Buttons[0]));
                row.Buttons[1].Button.onClick.AddListener(() => callback(row.Buttons[1]));
            }
        }

        [Button]
        [GUIColorDefaultButton]
        private void SetExample()
        {
        }

        public void SetPacks(ChestPackPayload[] chestPacks)
        {
            for (int i = 0; i < chestPacks.Length; i++)
            {
                ChestPanelRow row = m_rows[i];
                row.gameObject.SetActive(false);
            }

            // Assume that the size of both are matched!
            for (int i = 0; i < chestPacks.Length; i++)
            {
                ChestPackPayload chestPack = chestPacks[i];
                ChestPanelRow row = m_rows[i];
                row.Buttons[0].SetPackX(chestPack.PackXOne);
                row.Buttons[1].SetPackX(chestPack.PackXTen);

                ItemSheet item = Assets.LoadItemSheet(chestPack.CurrentChestId);
                row.SetTitleText(item.Name);
                row.SetUnlockNextChestProcess(chestPack.ObtainedCount, chestPack.UnlockNextChestAmount);

                if (chestPack.IsUnlocked)
                {
                    row.gameObject.SetActive(true);
                }
            }
        }
    }
}
