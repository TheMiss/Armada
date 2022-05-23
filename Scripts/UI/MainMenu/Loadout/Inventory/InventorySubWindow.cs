using System;
using System.Threading;
using Armageddon.Configuration;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule;
using Armageddon.UI.Common.InventoryModule.Slot;
using Armageddon.UI.Common.ItemInspectionModule;
using Armageddon.UI.Common.OpenChestModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.Inventory
{
    public class InventorySubWindow : SubWindow
    {
        [SerializeField]
        private HeroInventoryPanel m_heroInventoryPanel;
        
        [SerializeField]
        private PlayerInventoryPanel m_playerInventoryPanel;

        [SerializeField]
        private InventoryItemFilterBar m_playerInventoryItemFilterBar;

        [SerializeField]
        private Button m_switchHeroButton;

        [SerializeField]
        private RawImage m_heroPreviewDisplay;

        // Inventory
        private Item m_selectedAnotherItem;
        private int m_selectedEquippingSlotIndex;
        private Item m_selectedItem;
        private SelectState m_selectState;

        public HeroInventoryPanel HeroInventoryPanel => m_heroInventoryPanel;

        public PlayerInventoryPanel PlayerInventoryPanel => m_playerInventoryPanel;

        public InventoryItemFilterBar PlayerInventoryItemFilterBar => m_playerInventoryItemFilterBar;

        protected override void OnEnable()
        {
            base.OnEnable();

            var player = GetService<Player>();

            if (player == null)
            {
                return;
            }

            // Inventory
            HeroInventory heroInventory = player.HeroInventory;
            heroInventory.SlotSelected += OnSlotSelected;

            PlayerInventory playerInventory = player.PlayerInventory;
            playerInventory.SlotSelected += OnSlotSelected;

            m_switchHeroButton.onClick.AddListener(OnSwitchHeroButtonClicked);

            CanTick = true;

            ResetStates();
            // ResetAllSlots();

            Hero currentHero = UI.Game.Player.CurrentHero;
            UI.PreviewManager.ShowHeroAsync(this, currentHero,
                entry => m_heroPreviewDisplay.texture = entry.Camera.targetTexture).Forget();
        }

        protected override void OnDisable()
        {
            var player = GetService<Player>();

            if (player == null)
            {
                return;
            }

            // Inventory
            HeroInventory heroInventory = player.HeroInventory;
            heroInventory.SlotSelected -= OnSlotSelected;

            PlayerInventory playerInventory = player.PlayerInventory;
            playerInventory.SlotSelected -= OnSlotSelected;

            m_switchHeroButton.onClick.RemoveAllListeners();

            if (UI != null)
            {
                UI.InspectItemWindow.Hide(false);
            }

            UI.PreviewManager.HideHeroes(this);

            base.OnDisable();
        }

        private void ResetStates()
        {
            // Reset states of things...
            m_selectedAnotherItem = null;
            m_selectedEquippingSlotIndex = -1;

            m_selectState = SelectState.Nothing;
        }

        private void OnSlotSelected(object sender, SlotSelectedArgs e)
        {
            if (!(e.Object is Item item))
            {
                item = null;
            }

            Debug.Log($"LoadoutTabPage.OnSlotSelected: {e.Slot.Index}, clickCount = {e.ClickCount}");

            switch (m_selectState)
            {
                case SelectState.Nothing:
                {
                    if (item == null)
                    {
                        break;
                    }

                    // When double click
                    if (m_selectedItem == item && e.ClickCount == 2)
                    {
                        // If from HeroInventory we unequip.
                        if (sender is HeroInventory)
                        {
                            InventorySlot slot =
                                ((HeroInventory)HeroInventoryPanel.Inventory).GetEquippedSlot(item);

                            // Assume if reaches this point, we have the item in slot to unequip. But check it anyway
                            if (slot != null)
                            {
                                var ui = GetService<UISystem>();
                                ui.InspectItemWindow.Hide();

                                var slotType = (EquipmentSlotType)slot.Index;
                                UnequipHeroItemAsync(slotType).Forget();
                            }
                        }
                        // If from PlayerInventory we equip
                        else if (sender is PlayerInventory)
                        {
                            EnterEquipHeroItemStateAsync(m_selectedItem).Forget();
                        }

                        break;
                    }

                    m_selectedItem = item;
                    OpenInspectItemWindowAsync(item).Forget();

                    // Since we don't subscribe SlotStateChange we need to do it manually.
                    HeroInventoryPanel.AdjustSlotStates();
                    PlayerInventoryPanel.AdjustSlotStates();

                    // Make the selected slot widget standout.
                    SlotWidget slotWidget = HeroInventoryPanel.GetSlotWidget(item);
                    if (slotWidget != null)
                    {
                        slotWidget.AdjustState();
                    }
                    else
                    {
                        slotWidget = PlayerInventoryPanel.GetSlotWidget(item);

                        if (slotWidget != null)
                        {
                            slotWidget.AdjustState();
                        }
                    }

                    m_selectState = SelectState.Nothing;
                    break;
                }
                case SelectState.SelectEquippingSlot:
                {
                    // Ignore the same inspecting item.
                    // When select the another item to equip and then click on the same item is canceling.
                    if (item != m_selectedItem)
                    {
                        m_selectedEquippingSlotIndex = e.Slot.Index;
                    }
                    else
                    {
                        // -1 means cancelling.
                        m_selectedEquippingSlotIndex = -1;
                    }

                    m_selectState = SelectState.Nothing;
                    break;
                }
                case SelectState.SelectAnotherItemToCompare:
                {
                    m_selectedAnotherItem = item;
                    m_selectState = SelectState.Nothing;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTaskVoid OpenInspectItemWindowAsync(Item selectedItem)
        {
            bool isItemEquipped = HeroInventoryPanel.IsEquippingItem(selectedItem);

            InspectItemWindow window = UI.InspectItemWindow;
            // window.SetEquipButtonMode(!isItemBeingEquipped);
            window.SetOffsetY(isItemEquipped ? -1370 : -510);
            InspectItemWindowResult? result = await window.InspectAsync(selectedItem, isItemEquipped);

            switch (result)
            {
                case InspectItemWindowResult.Close:
                    break;
                case InspectItemWindowResult.Sell:
                {
                    SellItemAsync(selectedItem).Forget();

                    break;
                }
                case InspectItemWindowResult.Compare:
                {
                    Item anotherItem = await SelectItemToCompareAsync(selectedItem, selectedItem.Type);

                    // Canceled if the player selected the same item or another item's null
                    if (selectedItem == anotherItem || anotherItem == null)
                    {
                        return;
                    }

                    Debug.Log($"Compare {selectedItem.InstanceId} with {anotherItem.InstanceId}");
                    CompareItemsAsync(selectedItem, anotherItem).Forget();

                    break;
                }
                case InspectItemWindowResult.Equip:
                {
                    await EnterEquipHeroItemStateAsync(selectedItem);
                    break;
                }
                case InspectItemWindowResult.Unequip:
                {
                    InventorySlot slot = ((HeroInventory)HeroInventoryPanel.Inventory).GetEquippedSlot(selectedItem);

                    // Assume if reaches this point, we have the item in slot to unequip. But check it anyway
                    if (slot != null)
                    {
                        var slotType = (EquipmentSlotType)slot.Index;
                        UnequipHeroItemAsync(slotType).Forget();
                    }

                    break;
                }
                case InspectItemWindowResult.Use:
                {
                    var player = GetService<Player>();
                    await player.UseItemAsync(selectedItem, 1);
                    break;
                }
                case InspectItemWindowResult.Open:
                {
                    await OpenLootBoxWindowAsync((LootBox)selectedItem);
                    break;
                }
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ResetAllSlots();
        }

        private void ResetAllSlots()
        {
            HeroInventoryPanel.UnhighlightSlots();
            HeroInventoryPanel.HideSlotMarkers();
            HeroInventoryPanel.SetAllowSelectSlots(true);
            HeroInventoryPanel.DeselectAllSlots();

            PlayerInventoryPanel.UnhighlightSlots();
            PlayerInventoryPanel.HideSlotMarkers();
            PlayerInventoryPanel.SetAllowSelectSlots(true);
            PlayerInventoryPanel.DeselectAllSlots();

            m_selectedItem = null;
        }

        private async UniTask<bool> EnterEquipHeroItemStateAsync(Item selectedItem)
        {
            UI.InspectItemWindow.Hide();

            EquipmentSlotType? slotType;

            // If the target slots are non-selectable then equip immediately.
            if (selectedItem.Type == ItemType.PrimaryWeapon ||
                selectedItem.Type == ItemType.SecondaryWeapon ||
                selectedItem.Type == ItemType.Kernel ||
                selectedItem.Type == ItemType.Armor)
            {
                slotType = (EquipmentSlotType)selectedItem.Type;
            }
            // If not, let the player select.
            else
            {
                slotType = await SelectHeroInventorySlotsToEquipAsync(selectedItem);

                // Canceled if the player selected the same item
                if (slotType == null)
                {
                    return false;
                }
            }

            return await EquipHeroItemAsync(slotType.Value, selectedItem);
        }

        private async UniTask<bool> EquipHeroItemAsync(EquipmentSlotType slotType, Item item)
        {
            UI.WaitForServerResponse.Show();

            var playerInventory = (PlayerInventory)PlayerInventoryPanel.Inventory;
            var heroInventory = (HeroInventory)HeroInventoryPanel.Inventory;

            InventorySlot slotSource = playerInventory.GetSlot(item);
            InventorySlot slotTarget = heroInventory.GetSlotAt((int)slotType);

            await heroInventory.EquipItemAsync(slotSource, slotTarget);
            playerInventory.ShiftAllObjects();

            playerInventory.DeselectAll();
            heroInventory.DeselectAll();

            ResetAllSlots();

            UI.WaitForServerResponse.Hide();

            return true;
        }

        private async UniTaskVoid UnequipHeroItemAsync(EquipmentSlotType slotType)
        {
            var ui = GetService<UISystem>();
            ui.WaitForServerResponse.Show();

            var playerInventory = (PlayerInventory)PlayerInventoryPanel.Inventory;
            var heroInventory = (HeroInventory)HeroInventoryPanel.Inventory;

            InventorySlot slotSource = heroInventory.GetSlotAt((int)slotType);
            InventorySlot slotTarget = playerInventory.GetEmptySlot();

            await heroInventory.UnequipItemAsync(slotSource, slotTarget);
            playerInventory.ShiftAllObjects(); // Unnecessary but do it anyway.

            ResetAllSlots();

            ui.WaitForServerResponse.Hide();
        }

        private static async UniTaskVoid SellItemAsync(Item item)
        {
            throw new NotImplementedException(nameof(SellItemAsync));
            // var ui = GetService<UISystem>();
            // bool? dialogResult = await ui.ShopConfirmDialog.ShowSellAsync(item);
            //
            // if (dialogResult != null && dialogResult.Value)
            // {
            //     ui.WaitForServerResponse.Show();
            //
            //     var player = GetService<Player>();
            //     await player.SellItemAsync(item);
            //
            //     ui.WaitForServerResponse.Hide();
            // }
        }

        private async UniTaskVoid CompareItemsAsync(Item item1, Item item2)
        {
            var ui = GetService<UISystem>();
            InspectItemWindowResult? result = await ui.CompareItemWindow.CompareItemsModalAsync(item1, item2);

            switch (result)
            {
                case InspectItemWindowResult.Close:
                    break;
                case InspectItemWindowResult.Compare:
                {
                    Item selectedItem = ui.CompareItemWindow.SelectedItem;

                    if (HeroInventoryPanel.Inventory.GetSlot(selectedItem) != null)
                    {
                        HeroInventoryPanel.Inventory.SelectSlot(selectedItem, 1, false);
                    }
                    else if (PlayerInventoryPanel.Inventory.GetSlot(selectedItem) != null)
                    {
                        PlayerInventoryPanel.Inventory.SelectSlot(selectedItem, 1, false);
                    }

                    HeroInventoryPanel.AdjustSlotStates();
                    PlayerInventoryPanel.AdjustSlotStates();

                    Item anotherItem = await SelectItemToCompareAsync(selectedItem, selectedItem.Type);
                    // Debug.Log($"Compare {selectedItem.ServerId} with {anotherItem.ServerId}");

                    // Cancel if the player select the same item
                    if (selectedItem == anotherItem)
                    {
                        return;
                    }

                    // Do over with the selected item.
                    if (item1 == selectedItem)
                    {
                        CompareItemsAsync(selectedItem, anotherItem).Forget();
                    }
                    else if (item2 == selectedItem)
                    {
                        CompareItemsAsync(anotherItem, selectedItem).Forget();
                    }

                    break;
                }
                case InspectItemWindowResult.Equip:
                {
                    m_selectedItem = ui.CompareItemWindow.SelectedItem;
                    await EnterEquipHeroItemStateAsync(ui.CompareItemWindow.SelectedItem);
                }
                    break;
                case InspectItemWindowResult.Unequip:
                {
                    Item selectedItem = ui.CompareItemWindow.SelectedItem;
                    InventorySlot slot = ((HeroInventory)HeroInventoryPanel.Inventory).GetEquippedSlot(selectedItem);

                    // Assume if reaches this point, we have the item in slot to unequip. But check it anyway
                    if (slot != null)
                    {
                        var slotType = (EquipmentSlotType)slot.Index;
                        UnequipHeroItemAsync(slotType).Forget();
                    }
                }
                    break;
                case InspectItemWindowResult.Sell:
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ResetAllSlots();
        }

        private async UniTask<EquipmentSlotType?> SelectHeroInventorySlotsToEquipAsync(Item selectedItem)
        {
            HeroInventoryPanel.HighlightSlotsByItem(selectedItem, UISettings.DimSlotAlpha);
            HeroInventoryPanel.ShowSlotMarkersByItem(selectedItem);
            HeroInventoryPanel.SetAllowSelectSlotsByItem(selectedItem);

            PlayerInventoryPanel.HighlightSlot(selectedItem, UISettings.DimSlotAlpha);
            PlayerInventoryPanel.SetAllowSelectSlotByItem(selectedItem);
            PlayerInventoryPanel.LockScroll();

            m_selectState = SelectState.SelectEquippingSlot;
            CancellationToken token = GetCancellationToken(nameof(SelectHeroInventorySlotsToEquipAsync));

            while (m_selectState != SelectState.Nothing)
            {
                await UniTask.Yield(token);
            }

            HeroInventoryPanel.UnhighlightSlots();
            HeroInventoryPanel.HideSlotMarkers();

            PlayerInventoryPanel.UnhighlightSlots();
            PlayerInventoryPanel.SetAllowSelectSlots(true);
            PlayerInventoryPanel.UnlockScroll();

            // Yeah, we have canceled.
            if (m_selectedEquippingSlotIndex < 0)
            {
                return null;
            }

            return (EquipmentSlotType)m_selectedEquippingSlotIndex;
        }

        private async UniTask<Item> SelectItemToCompareAsync(Item selectedItem, ItemType itemType)
        {
            HeroInventoryPanel.AdjustSlotStates();
            PlayerInventoryPanel.AdjustSlotStates();

            HeroInventoryPanel.HighlightSlotsByItemType(itemType, UISettings.DimSlotAlpha);
            HeroInventoryPanel.ShowSlotMarkersByItemType(selectedItem);
            HeroInventoryPanel.SetAllowSelectSlotsByItem(selectedItem);

            PlayerInventoryPanel.HighlightSlotsByItemType(itemType, UISettings.DimSlotAlpha);
            PlayerInventoryPanel.ShowSlotMarkersByItemType(selectedItem);
            PlayerInventoryPanel.SetAllowSelectSlotsByItemType(selectedItem);

            m_selectState = SelectState.SelectAnotherItemToCompare;
            CancellationToken token = GetCancellationToken(nameof(SelectItemToCompareAsync));

            while (m_selectState != SelectState.Nothing)
            {
                await UniTask.Yield(token);
            }

            // TODO: Remove these since we will can ResetAllSlots() at the end of the caller anyway.
            HeroInventoryPanel.UnhighlightSlots();
            HeroInventoryPanel.HideSlotMarkers();
            HeroInventoryPanel.SetAllowSelectSlots(true);

            PlayerInventoryPanel.UnhighlightSlots();
            PlayerInventoryPanel.HideSlotMarkers();
            PlayerInventoryPanel.SetAllowSelectSlots(true);

            // HeroInventoryPanel.ResetAllSlotStates();
            // PlayerInventoryPanel.ResetAllSlotStates();

            return m_selectedAnotherItem;
        }

        private async UniTask OpenLootBoxWindowAsync(LootBox lootBox)
        {
            OpenChestWindow window = UI.OpenChestWindow;
            await window.ShowLootBoxAsync(lootBox);
        }

        private void OnSwitchHeroButtonClicked()
        {
            Hide();
            SubWindowManager.SetSelectedSubWindow((int)LoadoutSubpageType.SelectHero);
            UI.TabPageBottomBar.ShowWithOptions();
            UI.MainMenuBar.Hide();

            // // Glitch in Hide animation
            // // m_inventoryPanel.HideAndNotify(() =>
            // // {
            // //      m_selectHeroPanel.Show();
            // // });
            //
            // CurrentPanelType = LoadoutTabPagePanelType.SelectHero;
            //
            // var ui = GetService<UISystem>();
            // ui.MainMenuBar.Hide();
            //
            // m_inventorySubpage.Hide(false);
            //
            // // Select Hero
            // m_selectHeroSubpage.Show();
            // m_inspectHeroWindow.Show(false);
            // m_heroListWindow.Show(false);
            // m_heroListWindow.FilterBar.Show(false);
            //
            // // Requires m_heroListWindow to be set up first.
            // m_inspectHeroWindow.SetSelectedHero();
        }

        public override void Tick()
        {
#if DEBUG
            // All moved to CheatCode itself
#endif
        }

        private enum SelectState
        {
            Nothing,
            SelectEquippingSlot,
            SelectAnotherItemToCompare
        }
    }
}
