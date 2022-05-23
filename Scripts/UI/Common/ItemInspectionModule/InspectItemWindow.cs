using System;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Bonuses;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Stats;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using Armageddon.Worlds;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using ParadoxNotion;
using Purity.Common;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.ItemInspectionModule
{
    public enum InspectItemWindowResult
    {
        Close,
        Sell,
        Compare,
        Equip,
        Unequip,
        Use,
        Open
    }

    public enum InspectItemWindowMode
    {
        InInventory,
        InShop
    }

    public class InspectItemWindow : Window
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private AbilityPanel m_abilityPanelPrefab;

        [SerializeField]
        private TextMeshProUGUI m_itemTitleText;

        [SerializeField]
        private TextMeshProUGUI m_itemQualityText;

        [SerializeField]
        private TextMeshProUGUI m_itemDescriptionText;

        [SerializeField]
        private List<AbilityPanel> m_abilityPanels;

        [SerializeField]
        private TextMeshProUGUI m_detailsText;

        [SerializeField]
        private ObjectHolderItem m_objectHolderItem;

        [SerializeField]
        private Button m_closeButton;

        [SerializeField]
        private Button m_sellButton;

        [SerializeField]
        private Button m_compareButton;

        [SerializeField]
        private Button m_equipButton;

        [SerializeField]
        private Button m_unequipButton;

        [SerializeField]
        private Button m_useButton;

        [SerializeField]
        private Button m_openButton;

        [SerializeField]
        private Toggle m_useToggle;

        [SerializeField]
        private GameObject m_topPanelObject;

        [SerializeField]
        private GameObject m_middlePanelObject;

        [SerializeField]
        private GameObject m_bottomPanelObject;

        [SerializeField]
        private GameObject m_weaponStatsObject;

        [SerializeField]
        private GameObject m_kernelStatsObject;

        [SerializeField]
        private GameObject m_armorStatsObject;

        [SerializeField]
        private GameObject m_accessoryStatsObject;

        [SerializeField]
        private GameObject m_companionStatsObject;

        [SerializeField]
        private ItemMainStatRow m_dpsRow;

        [SerializeField]
        private ItemMainStatRow m_damageRow;

        [SerializeField]
        private ItemMainStatRow m_fireRateRow;

        [SerializeField]
        private ItemMainStatRow m_dexterityRow;

        [SerializeField]
        private ItemMainStatRow m_vitalityRow;

        [SerializeField]
        private ItemMainStatRow m_perceptionRow;

        [SerializeField]
        private ItemMainStatRow m_leadershipRow;

        [SerializeField]
        private ItemMainStatRow m_armorRow;

        [SerializeField]
        private ItemMainStatRow m_companionDpsRow;

        [SerializeField]
        private ItemMainStatRow m_companionDamageRow;

        [SerializeField]
        private ItemMainStatRow m_companionFireRateRow;

        [SerializeField]
        private ItemMainStatRow m_companionCriticalChanceRow;

        [SerializeField]
        private ItemMainStatRow m_companionCriticalDamageRow;

        [SerializeField]
        private RectTransform m_detailsContentTransform;

        [SerializeField]
        private Toggle m_previewToggle;

        [SerializeField]
        private Widget m_heroPreviewWidget;

        [SerializeField]
        private RawImage m_previewDisplay;

        [SerializeField]
        private List<GameObject> m_colorizableParts;

        private Item m_item;

        private List<ItemMainStatRow> m_mainStatRows;

        private List<GameObject> m_mainStatsObjects;

        private List<ItemMainStatRow> MainStatRows =>
            m_mainStatRows ??= GetComponentsInChildren<ItemMainStatRow>(true).ToList();

        private List<Component> BottomComponents =>
            new()
            {
                m_equipButton,
                m_unequipButton,
                m_sellButton,
                m_compareButton,
                m_useButton,
                m_openButton,
                m_useToggle
            };

        private List<GameObject> MainStatsObjects => m_mainStatsObjects ??= new List<GameObject>
        {
            m_weaponStatsObject, m_kernelStatsObject, m_armorStatsObject, m_accessoryStatsObject, m_companionStatsObject
        };

        [ShowInPlayMode]
        public string PreviewLayerName { get; set; }

        /// <summary>
        ///     Automatically return to InInventory mode after showing
        /// </summary>
        [ShowInPlayMode]
        public InspectItemWindowMode Mode { get; set; }

        public InspectItemWindowResult? WindowResult { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Right Side
            m_heroPreviewWidget.IsActive = false;
            m_previewToggle.isOn = false;
            m_previewToggle.onValueChanged.AddListener(OnPreviewToggleChanged);

            // Bottom
            m_closeButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Close));
            m_sellButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Sell));
            m_compareButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Compare));
            m_equipButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Equip));
            m_unequipButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Unequip));
            m_useButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Use));
            m_openButton.onClick.AddListener(() => OnBottomButtonClicked(InspectItemWindowResult.Open));
            m_useToggle.onValueChanged.AddListener(OnUseToggleChanged);
        }

        protected override void OnDisable()
        {
            m_previewToggle.onValueChanged.RemoveAllListeners();

            m_closeButton.onClick.RemoveAllListeners();
            m_sellButton.onClick.RemoveAllListeners();
            m_compareButton.onClick.RemoveAllListeners();
            m_equipButton.onClick.RemoveAllListeners();
            m_unequipButton.onClick.RemoveAllListeners();
            m_useButton.onClick.RemoveAllListeners();
            m_openButton.onClick.RemoveAllListeners();
            m_useToggle.onValueChanged.RemoveAllListeners();

            UI.PreviewManager.HideHeroes(this);

            base.OnDisable();
        }

        private void OnPreviewButtonClicked()
        {
            m_heroPreviewWidget.ToggleState();
        }

        private void OnPreviewToggleChanged(bool value)
        {
            if (value)
            {
                m_heroPreviewWidget.Show();
                ShowHeroPreview().Forget();
            }
            else
            {
                m_heroPreviewWidget.Hide();
            }
        }

        private async UniTask ShowHeroPreview()
        {
            Hero currentHero = UI.Game.Player.CurrentHero;
            PreviewEntry entry = await UI.PreviewManager.ShowHeroAsync(this, currentHero);
            HeroActor heroActor = entry.HeroActor;
            await heroActor.EquipItemAsync(EquipmentSlotType.PrimaryWeapon, m_item);
            heroActor.SetFiring(true);
            heroActor.SetCameraForWeapons(entry.Camera);

            m_previewDisplay.texture = entry.Camera.targetTexture;
        }

        private void OnBottomButtonClicked(InspectItemWindowResult result)
        {
            switch (result)
            {
                case InspectItemWindowResult.Close:
                    DialogResult = false;
                    break;
                case InspectItemWindowResult.Sell:
                case InspectItemWindowResult.Compare:
                case InspectItemWindowResult.Equip:
                case InspectItemWindowResult.Unequip:
                case InspectItemWindowResult.Use:
                case InspectItemWindowResult.Open:
                    DialogResult = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }

            WindowResult = result;
        }

        private void OnUseToggleChanged(bool value)
        {
            if (!m_item.IsUsable)
            {
                return;
            }

            m_item.UseWhenStartGame = value;
            Debug.Log($"{m_item.Name} is {(value ? "" : "not")} used when start game.");
        }

        public static bool? ToDialogResult(InspectItemWindowResult? windowResult)
        {
            switch (windowResult)
            {
                case InspectItemWindowResult.Close:
                    return false;
                case InspectItemWindowResult.Sell:
                case InspectItemWindowResult.Compare:
                case InspectItemWindowResult.Equip:
                case InspectItemWindowResult.Unequip:
                case InspectItemWindowResult.Use:
                case InspectItemWindowResult.Open:
                    return true;
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(windowResult), windowResult, null);
            }
        }

        public void SetAsAnother()
        {
            m_closeButton.gameObject.SetActive(false);
            // m_bottomPanelObject.SetActive(false);
        }

        private void DisableAllBottomComponents()
        {
            foreach (Component component in BottomComponents)
            {
                component.gameObject.SetActive(false);
            }
        }

        private void SetEquipButtonMode(bool showEquipButton)
        {
            DisableAllBottomComponents();

            if (showEquipButton)
            {
                m_equipButton.gameObject.SetActive(true);
                m_sellButton.gameObject.SetActive(true);
                m_compareButton.gameObject.SetActive(true);
            }
            else
            {
                m_unequipButton.gameObject.SetActive(true);
            }
        }

        public void SetUseButtonMode()
        {
            DisableAllBottomComponents();

            m_useButton.gameObject.SetActive(true);
        }

        public void SetOpenButtonMode()
        {
            DisableAllBottomComponents();

            m_openButton.gameObject.SetActive(true);
        }

        public void SetUseWhenStartGameButtonMode(bool toggleOn)
        {
            DisableAllBottomComponents();

            m_useToggle.gameObject.SetActive(true);
            m_useToggle.isOn = toggleOn;
        }

        private void SetInShopMode()
        {
            DisableAllBottomComponents();
        }

        public void SetOffsetY(float offset)
        {
            Vector2 anchoredPosition = RectTransform.anchoredPosition;
            anchoredPosition.y = offset;
            RectTransform.anchoredPosition = anchoredPosition;
        }

        public async UniTask<InspectItemWindowResult?> InspectAsync(Item item, bool isItemEquipped,
            Action<InspectItemWindowResult?> resultCallback = null)
        {
            if (Mode == InspectItemWindowMode.InInventory)
            {
                if (item.IsEquipable)
                {
                    SetEquipButtonMode(!isItemEquipped);
                }
                else if (item.IsUsable)
                {
                    if (item.Type == ItemType.Consumable)
                    {
                        SetUseWhenStartGameButtonMode(item.UseWhenStartGame);
                    }
                    else if (item.Type == ItemType.Chest)
                    {
                        SetOpenButtonMode();
                    }
                    else
                    {
                        SetUseButtonMode();
                    }
                }
            }
            else if (Mode == InspectItemWindowMode.InShop)
            {
                SetInShopMode();
                Mode = InspectItemWindowMode.InInventory;
            }

            SetItem(item);
            WindowResult = null;

            await ShowDialogAsync(true, false);

            resultCallback?.Invoke(WindowResult);

            return WindowResult;
        }

        public void SetItem(Item item)
        {
            m_item = item;

            m_itemTitleText.Set($"{m_item.Name}");
            m_itemQualityText.Set($"{m_item.Quality}");

            SetDetails(item);

            m_objectHolderItem.Initialize(item);
            m_objectHolderItem.ShowBottomRight(false);

            bool isPreviewActive =
                m_item.Sheet.Type == ItemType.PrimaryWeapon ||
                m_item.Sheet.Type == ItemType.SecondaryWeapon;

            m_previewToggle.gameObject.SetActive(isPreviewActive);
        }

        private void SetDetails(Item item)
        {
            string text = string.Empty;

            // TODO: Localize

            // foreach (ItemMainStatRow statRow in MainStatRows)
            // {
            //     statRow.gameObject.SetActive(false);    
            // }

            foreach (GameObject mainStatsObject in MainStatsObjects)
            {
                mainStatsObject.SetActive(false);
            }

            if (item is Weapon weapon)
            {
                m_weaponStatsObject.SetActive(true);

                m_dpsRow.SetStatValue($"{weapon.DamagePerSecond.AsDps()}");
                m_damageRow.SetStatValue($"{(long)weapon.DamagePerShot.Value}x{weapon.ShotsPerFire.Value}");
                m_fireRateRow.SetStatValue($"{weapon.FireRate.AsFireRate()}");
            }
            else if (item is Kernel kernel)
            {
                m_kernelStatsObject.SetActive(true);

                m_dexterityRow.SetStatValue($"{kernel.DexterityMultiplier.AsStatMultiplier()}");
                m_vitalityRow.SetStatValue($"{kernel.VitalityMultiplier.AsStatMultiplier()}");
                m_perceptionRow.SetStatValue($"{kernel.PerceptionMultiplier.AsStatMultiplier()}");
                m_leadershipRow.SetStatValue($"{kernel.LeadershipMultiplier.AsStatMultiplier()}");
            }
            else if (item is Armor armor)
            {
                m_armorStatsObject.SetActive(true);

                m_armorRow.SetStatValue($"{(long)armor.ArmorBonus.Value}");
            }
            else if (item is Companion companion)
            {
                m_companionStatsObject.SetActive(true);

                m_companionDpsRow.SetStatValue($"{companion.DamagePerSecond.AsDps()}");
                m_companionDamageRow.SetStatValue(
                    $"{(long)companion.DamagePerShot.Value}x{companion.ShotsPerFire.Value}");
                m_companionFireRateRow.SetStatValue($"{companion.FireRate.AsFireRate()}");
                m_companionCriticalChanceRow.SetStatValue($"{companion.CriticalChance.AsCriticalChance()}");
                m_companionCriticalDamageRow.SetStatValue($"{companion.CriticalDamage.AsCriticalDamage()}");
            }

            foreach (AbilityPanel abilityPanel in m_abilityPanels)
            {
                abilityPanel.gameObject.SetActive(false);
            }

            int abilityPanelIndex = 0;

            List<Bonus> bonuses = GetSortedBonuses(item);

            foreach (Bonus bonus in bonuses)
            {
                if (bonus is StatBonus statBonus)
                {
                    string sign = statBonus.Value > 0 ? "+" : "-";
                    string percent = GetPercentString(statBonus);
                    text += $"{sign}{statBonus.Value}{percent} {statBonus.StatId}\n";
                }
                else if (bonus is AbilityBonus abilityBonus)
                {
                    if (abilityPanelIndex >= m_abilityPanels.Count)
                    {
                        Debug.LogError("abilityPanelIndex >= m_abilityPanels.Count. Please add more panels.");
                        continue;
                    }

                    AbilityPanel abilityPanel = m_abilityPanels[abilityPanelIndex++];
                    abilityPanel.gameObject.SetActive(true);
                    string titleText = $"Passive: {abilityBonus.Sheet.Name} Lv. {abilityBonus.Level}";
                    abilityPanel.SetTitleText(titleText);
                    abilityPanel.SetDetailsText("Do some shit here");

                    // const string sign = "+";
                    //
                    // text += $"{sign}{abilityBonus.Level} {abilityBonus.Sheet.Name}\n";
                }
            }

            m_detailsText.Set(text);
            m_itemDescriptionText.Set("The weapon you will love using everyday! But you should be careful though");
            SetQuality(item.Quality);
        }

        private List<Bonus> GetSortedBonuses(Item item)
        {
            var statBonuses = new List<StatBonus>();
            var abilityBonuses = new List<AbilityBonus>();

            foreach (Bonus bonus in item.Bonuses)
            {
                switch (bonus)
                {
                    case StatBonus statBonus:
                        statBonuses.Add(statBonus);
                        break;
                    case AbilityBonus abilityBonus:
                        abilityBonuses.Add(abilityBonus);
                        break;
                }
            }

            statBonuses.Sort((x, y) => x.StatId < y.StatId ? -1 : 1);
            abilityBonuses.Sort((x, y) => x.Sheet.Id < y.Sheet.Id ? -1 : 1);

            var bonuses = new List<Bonus>();
            bonuses.AddRange(statBonuses);
            bonuses.AddRange(abilityBonuses);

            return bonuses;
        }

        private string GetPercentString(StatBonus statBonus)
        {
            if (statBonus.StatId == StatId.CriticalChance ||
                statBonus.StatId == StatId.CriticalDamage ||
                statBonus.StatId == StatId.CriticalResistance)
            {
                return "%";
            }

            return statBonus.ModifierType == StatModifierType.Percentage ? "%" : string.Empty;
        }

        [Button("Common")]
        [ButtonGroup("ItemQuality")]
        [GUIColorCommon]
        private void SetQualityCommon()
        {
            SetQuality(ItemQuality.Common);
        }

        [Button("Uncommon")]
        [ButtonGroup("ItemQuality")]
        [GUIColorUncommon]
        private void SetQualityUncommon()
        {
            SetQuality(ItemQuality.Uncommon);
        }

        [Button("Rare")]
        [ButtonGroup("ItemQuality")]
        [GUIColorRare]
        private void SetQualityRare()
        {
            SetQuality(ItemQuality.Rare);
        }

        [Button("Epic")]
        [ButtonGroup("ItemQuality")]
        [GUIColorEpic]
        private void SetQualityEpic()
        {
            SetQuality(ItemQuality.Epic);
        }

        [Button("Legendary")]
        [ButtonGroup("ItemQuality")]
        [GUIColorLegendary]
        private void SetQualityLegendary()
        {
            SetQuality(ItemQuality.Legendary);
        }

        [Button("Immortal")]
        [ButtonGroup("ItemQuality")]
        [GUIColorImmortal]
        private void SetQualityImmortal()
        {
            SetQuality(ItemQuality.Immortal);
        }

        [Button("Ancient")]
        [ButtonGroup("ItemQuality")]
        [GUIColorAncient]
        private void SetQualityAncient()
        {
            SetQuality(ItemQuality.Ancient);
        }

        private void SetQuality(ItemQuality itemQuality)
        {
            var color = itemQuality.ToColor();

            foreach (GameObject part in m_colorizableParts)
            {
                var image = part.GetComponent<Image>();
                if (image != null)
                {
                    image.color = color.WithAlpha(image.color.a);
                }

                // Image[] images = part.GetComponentsInChildren<Image>();
                //
                // foreach (Image childImage in images)
                // {
                //     childImage.color = color.WithAlpha(childImage.color.a);
                // }

                var text = part.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.color = color.WithAlpha(text.color.a);
                }

                // TextMeshProUGUI[] texts = part.GetComponentsInChildren<TextMeshProUGUI>();
                //
                // foreach (TextMeshProUGUI childText in texts)
                // {
                //     childText.color = color.WithAlpha(childText.color.a);
                // }
            }
        }
    }
}
