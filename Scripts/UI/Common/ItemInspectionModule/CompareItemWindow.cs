using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Armageddon.UI.MainMenu.Loadout;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.UI.Common.ItemInspectionModule
{
    public class CompareItemWindow : Window
    {
        [SerializeField]
        private InspectItemWindow m_inspector1;

        [SerializeField]
        private InspectItemWindow m_inspector2;

        public InspectItemWindow Inspector1 => m_inspector1;

        public InspectItemWindow Inspector2 => m_inspector2;

        public InspectItemWindowResult? WindowResult { private set; get; }
        public Item SelectedItem { private set; get; }
        public InspectItemWindow SelectedWindow { private set; get; }

        public async UniTask<InspectItemWindowResult?> CompareItemsModalAsync(Item item1, Item item2)
        {
            // Well the CompareItemWindow itself will control 2 inspect item windows.
            // let's activate and let it be.
            gameObject.SetActive(true);

            AddBlocker(null, new Color(0, 0, 0, 240 / 255f), () =>
            {
                WindowResult = InspectItemWindowResult.Close;
                Inspector1.Hide();
                Inspector2.Hide();
            });

            // Assume much that this won't fail, might fix this to a proper way...
            var loadoutTabPage = (LoadoutTabWindow)UI.MainMenuScreen.TabBar.CurrentTabWindow;

            bool isItem1Equipped = loadoutTabPage.InventorySubWindow.HeroInventoryPanel.IsEquippingItem(item1);
            // Inspector1.SetEquipButtonMode(!isItem1Equipped);
            Inspector1.InspectAsync(item1, isItem1Equipped, result =>
            {
                WindowResult = result;
                SelectedItem = item1;
                SelectedWindow = Inspector1;
                DialogResult = InspectItemWindow.ToDialogResult(result);

                Inspector2.Hide();
            }).Forget();

            bool isItem2Equipped = loadoutTabPage.InventorySubWindow.HeroInventoryPanel.IsEquippingItem(item2);
            // Inspector2.SetEquipButtonMode(!isItem2Equipped);
            Inspector2.InspectAsync(item2, isItem2Equipped, result =>
            {
                WindowResult = result;
                SelectedItem = item2;
                SelectedWindow = Inspector2;
                DialogResult = InspectItemWindow.ToDialogResult(result);

                Inspector1.Hide();
            }).Forget();

            WindowResult = null;

            while (WindowResult == null)
            {
                await UniTask.Yield();
            }

            RemoveBlocker();

            return WindowResult;
        }
    }
}
