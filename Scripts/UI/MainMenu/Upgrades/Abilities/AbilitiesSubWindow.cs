using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Abilities;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Upgrades.Abilities
{
    public class AbilitiesSubWindow : SubWindow
    {
        [SerializeField]
        private AbilityElement m_abilityElementPrefab;

        [SerializeField]
        private ScrollRect m_abilitiesScrollRect;

        [SerializeField]
        private RectTransform m_abilitiesContentTransform;

        [SerializeField]
        private AbilityTooltip m_abilityTooltip;

        [SerializeField]
        private UpgradeAbilityButton m_upgradeAbilityButton;

        [SerializeField]
        private TextMeshProUGUI m_upgradeAbilityText;

        private List<AbilityElement> m_abilityButtons;

        private bool m_shouldUpdateScrollRectSettings;
        private AbilityElement m_selectedAbilityElement;

        protected override void Awake()
        {
            base.Awake();
            
            m_upgradeAbilityButton.gameObject.SetActive(false);
            m_upgradeAbilityButton.onClick.AddListener(OnUpgradeAbilityButtonClicked);
            m_upgradeAbilityButton.PointerExited.AddListener(OnUpgradeAbilityButtonPointerExited);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Initialize
            if (m_abilityButtons == null)
            {
                m_abilityButtons = new List<AbilityElement>();
                m_abilitiesContentTransform.DestroyDesignRemnant();
            }

            List<PlayerAbility> playerAbilities = UI.Game.Player.Abilities;

            foreach (PlayerAbility playerAbility in playerAbilities)
            {
                AbilityElement abilityElement = m_abilityButtons.Find(x => x.PlayerAbility == playerAbility);

                if (abilityElement == null)
                {
                    abilityElement = Instantiate(m_abilityElementPrefab, m_abilitiesContentTransform);
                    abilityElement.PlayerAbility = playerAbility;
                    abilityElement.Selected.AddListener(OnAbilityElementSelected);
                    abilityElement.Deselected.AddListener(OnAbilityElementDeselected);

                    m_abilityButtons.Add(abilityElement);
                }

                abilityElement.UpdateDetails();
            }

            UI.Game.Player.CompileAbilitiesFromVariousSources();

            m_shouldUpdateScrollRectSettings = true;
            m_abilityTooltip.gameObject.SetActive(false);
            m_selectedAbilityElement = null;

            CanTick = true;
        }

        private void OnAbilityElementSelected(AbilityElement abilityElement)
        {
            var elementRectTransform = abilityElement.GetComponent<RectTransform>();
            var tooltipRectTransform = m_abilityTooltip.GetComponent<RectTransform>();
            Vector3 position = abilityElement.Transform.position;
            float offsetX = elementRectTransform.rect.width * 0.5f + tooltipRectTransform.rect.width * 0.5f;
            position.x += offsetX;

            m_abilityTooltip.SetArrowAtLeftSide();
            var rootRectTransform = UI.GetComponent<RectTransform>();

            if (position.x + tooltipRectTransform.rect.width * 0.5f > rootRectTransform.rect.width)
            {
                position.x = abilityElement.Transform.position.x - offsetX;
                m_abilityTooltip.SetArrowAtRightSide();
            }

            m_abilityTooltip.gameObject.SetActive(true);
            m_abilityTooltip.Transform.position = position;
            m_abilityTooltip.SetAbility(abilityElement.PlayerAbility);

            UpdateUpgradeAbilityButton(abilityElement);

            m_selectedAbilityElement = abilityElement;
        }

        private void OnAbilityElementDeselected(AbilityElement abilityElement)
        {
            if (!m_upgradeAbilityButton.IsPointerOver)
            {
                m_abilityTooltip.gameObject.SetActive(false);
                m_upgradeAbilityButton.gameObject.SetActive(false);
            }

            Debug.Log("OnAbilityButtonDeselected");
        }

        private void UpdateUpgradeAbilityButton(AbilityElement abilityElement)
        {
            if (abilityElement.PlayerAbility.HasNextUpgrade)
            {
                string text = $"{Texts.UI.Upgrade}\n";
                text += CurrencyType.GoldShard.ToSpriteCode(abilityElement.PlayerAbility.UpgradePrice.Amount);

                m_upgradeAbilityText.Set(text);
                m_upgradeAbilityButton.gameObject.SetActive(true);
            }
            else
            {
                m_upgradeAbilityButton.gameObject.SetActive(false);
            }

            m_abilityTooltip.SetAbility(abilityElement.PlayerAbility);
        }

        private void OnUpgradeAbilityButtonClicked()
        {
            UpgradePlayerAbilityAsync().Forget();
        }

        private void OnUpgradeAbilityButtonPointerExited()
        {
            if (m_selectedAbilityElement != null)
            {
                EventSystem.SetSelectedGameObject(m_selectedAbilityElement.gameObject);
            }
        }

        private async UniTaskVoid UpgradePlayerAbilityAsync()
        {
            await UpgradePlayerAbilityAsync(m_selectedAbilityElement.PlayerAbility);

            UpdateUpgradeAbilityButton(m_selectedAbilityElement);
            m_selectedAbilityElement.UpdateDetails();
        }

        private async UniTask UpgradePlayerAbilityAsync(PlayerAbility playerAbility)
        {
            var player = GetService<Player>();

            if (!player.CanUpgradeAbility(playerAbility, out string reason))
            {
                // TODO: Localize titleText
                string titleText = string.Empty;
                string acceptButtonText = Texts.UI.GotIt;

                UI.AlertDialog.ShowInfoDialogAsync(titleText, reason, acceptButtonText).Forget();
                return;
            }

            UI.WaitForServerResponse.Show();

            await player.UpgradeAbilityAsync(playerAbility);
            player.CompileAbilitiesFromVariousSources();

            UI.WaitForServerResponse.Hide();
        }

        private void UpdateScrollRectSettings()
        {
            var parentRectTransform = m_abilitiesContentTransform.parent.GetComponent<RectTransform>();

            // if (m_abilitiesContentTransform.rect.height > parentRectTransform.rect.height)
            // {
            //     m_abilitiesScrollRect.vertical = true;
            // }
            // else
            // {
            //     m_abilitiesScrollRect.vertical = false;
            // }
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                List<PlayerAbility> playerAbilities = UI.Game.Player.Abilities;

                foreach (PlayerAbility playerAbility in playerAbilities)
                {
                    // for (int i = 0; i < 8; i++)
                    {
                        AbilityElement abilityElement =
                            Instantiate(m_abilityElementPrefab, m_abilitiesContentTransform);
                        abilityElement.PlayerAbility = playerAbility;
                        abilityElement.Selected.AddListener(OnAbilityElementSelected);
                        abilityElement.Deselected.AddListener(OnAbilityElementDeselected);
                        abilityElement.UpdateDetails();

                        m_abilityButtons.Add(abilityElement);
                    }
                }

                UpdateScrollRectSettings();
            }
        }

        public override void LateTick()
        {
            if (m_shouldUpdateScrollRectSettings)
            {
                UpdateScrollRectSettings();

                m_shouldUpdateScrollRectSettings = false;
            }
        }
    }
}
