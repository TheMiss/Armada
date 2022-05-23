using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Configuration;
using Armageddon.Mechanics;
using Armageddon.Sheets;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common
{
    public class TopBar : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_playerLevelText;

        [SerializeField]
        private Image m_playerExpProgressBarFiller;

        [SerializeField]
        private List<CurrencyBar> m_currencyBars;

        private bool m_hasInitialAnchoredPosition;

        [ShowInPlayMode]
        private Vector2 m_initialAnchoredPosition;

        public IReadOnlyList<CurrencyBar> CurrencyBars => m_currencyBars;

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (CurrencyBar currencyBar in CurrencyBars)
            {
                currencyBar.Clicked.AddListener(OnCurrencyBarClicked);
            }
        }

        protected override void OnDisable()
        {
            foreach (CurrencyBar currencyBar in CurrencyBars)
            {
                currencyBar.Clicked.RemoveListener(OnCurrencyBarClicked);
            }

            base.OnDisable();
        }

        public override void Show(bool animate = true)
        {
            if (!m_hasInitialAnchoredPosition)
            {
                m_initialAnchoredPosition = RectTransform.anchoredPosition;
                m_hasInitialAnchoredPosition = true;
            }

            Rect rect = RectTransform.rect;
            Vector2 anchoredPosition = RectTransform.anchoredPosition;
            anchoredPosition.y += rect.height + 10;
            RectTransform.anchoredPosition = anchoredPosition;


            DoTweenAnimation = () => DOTween.To(() => RectTransform.anchoredPosition.y,
                y => RectTransform.anchoredPosition = new Vector2(m_initialAnchoredPosition.x, y),
                m_initialAnchoredPosition.y, UISettings.TopBarAnimationDuration).OnComplete(OnOpenAnimationFinished);

            base.Show(animate);

            foreach (CurrencyBar currencyBar in CurrencyBars)
            {
                currencyBar.Show(animate);
            }
        }

        public override void Hide(bool animate = true)
        {
            if (!m_hasInitialAnchoredPosition)
            {
                return;
            }

            // The process is reversed from Show
            Rect rect = RectTransform.rect;
            Vector2 anchoredPosition = RectTransform.anchoredPosition;
            anchoredPosition.y += rect.height + 10;
            RectTransform.anchoredPosition = m_initialAnchoredPosition;

            DoTweenAnimation = () => DOTween.To(() => RectTransform.anchoredPosition.y,
                y => RectTransform.anchoredPosition = new Vector2(m_initialAnchoredPosition.x, y),
                anchoredPosition.y, UISettings.TopBarAnimationDuration).OnComplete(OnCloseAnimationFinished);

            base.Hide(animate);
        }

        private void OnCurrencyBarClicked(CurrencyType currencyType)
        {
            var ui = GetService<UISystem>();

            if (currencyType == CurrencyType.EvilHeart)
            {
                ui.AlertDialog.ShowInfoDialogAsync("Fun Fact!",
                    "Blue Gem can be obtained from quests, a special shop, and events", "Got it!").Forget();
            }
            else
            {
                // TODO: Change Text
                ui.AlertDialog.ShowInfoDialogAsync("Shop offers!", "Hey this is a test text!", "Understood!").Forget();
            }
        }

        public void SetPlayerDetails(Player player)
        {
            // TODO: Localize
            m_playerLevelText.Set($"Lv. {player.Level}");

            ExpTable expTable = Assets.LoadExpTable();
            expTable.GetRow(player.Level, out ExpTableDetailsRow currentLevelRow);
            float fillAmount = 1.0f;
            if (currentLevelRow.ExpForNextLevel > 0)
            {
                long expDelta = player.Exp - currentLevelRow.TotalExp;
                fillAmount = (float)((double)expDelta / currentLevelRow.ExpForNextLevel);
            }

            m_playerExpProgressBarFiller.fillAmount = fillAmount;
        }
    }
}
