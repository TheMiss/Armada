using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Extensions;
using Armageddon.Mechanics;
using Armageddon.Sheets.Items;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common;
using UnityEngine;

namespace Armageddon.UI.MainMenu.Upgrades.Powers
{
    public class PowersSubWindow : SubWindow
    {
        public static bool LogCreateCardButtonsTimeTaken;
        
        [SerializeField]
        private PowerElement m_powerElementPrefab;

        [SerializeField]
        private RectTransform m_powersContentTransform;

        [SerializeField]
        private PowerTooltip m_powerTooltip;

        private List<PowerElement> m_powerButtons;
        private PowerElement m_selectedPowerElement;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Initialize
            if (m_powerButtons == null)
            {
                
                CreateCardButtonsAsync().Forget();
            }
            else
            {
                UpdatePowerButtons();
            }

            m_powerTooltip.gameObject.SetActive(false);
        }

        private async UniTask CreateCardButtonsAsync()
        {
            var stopWatch = new Stopwatch("CreateCardButtonsAsync");

            m_powersContentTransform.DestroyDesignRemnant();
            m_powerButtons = new List<PowerElement>();
            
            List<CardSheet> cardSheets = await Assets.LoadCardSheetsAsync();

            foreach (CardSheet cardSheet in cardSheets)
            {
                PowerElement powerElement = Instantiate(m_powerElementPrefab, m_powersContentTransform);
                powerElement.CardSheet = cardSheet;
                powerElement.Selected.AddListener(OnPowerButtonSelected);
                powerElement.Deselected.AddListener(OnPowerButtonDeselected);

                m_powerButtons.Add(powerElement);
            }

            UpdatePowerButtons();

            stopWatch.Stop(LogCreateCardButtonsTimeTaken);
        }

        private void OnPowerButtonSelected(PowerElement powerElement)
        {
            var buttonRectTransform = powerElement.GetComponent<RectTransform>();
            var tooltipRectTransform = m_powerTooltip.GetComponent<RectTransform>();
            Vector3 position = powerElement.Transform.position;
            float offsetX = buttonRectTransform.rect.width * 0.5f + tooltipRectTransform.rect.width * 0.5f;
            position.x += offsetX;

            m_powerTooltip.SetArrowAtLeftSide();
            var rootRectTransform = UI.GetComponent<RectTransform>();

            if (position.x + tooltipRectTransform.rect.width * 0.5f > rootRectTransform.rect.width)
            {
                position.x = powerElement.Transform.position.x - offsetX;
                m_powerTooltip.SetArrowAtRightSide();
            }

            m_powerTooltip.Transform.position = position;
            m_powerTooltip.gameObject.SetActive(true);
            m_powerTooltip.SetPower(powerElement.CardSheet);

            m_selectedPowerElement = powerElement;
        }

        private void OnPowerButtonDeselected(PowerElement powerElement)
        {
            m_powerTooltip.gameObject.SetActive(false);
        }

        private void UpdatePowerButtons()
        {
            var player = GetService<Player>();

            foreach (PowerElement powerButton in m_powerButtons)
            {
                powerButton.IsPowerActive = false;

                CardPower foundCardPower = player.CardPowers.Find(x => x.Sheet == powerButton.CardSheet);

                if (foundCardPower != null)
                {
                    powerButton.IsPowerActive = true;
                }
            }
        }
    }
}
