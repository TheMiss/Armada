using Armageddon.Backend.Payloads;
using Armageddon.Configuration;
using Armageddon.Games;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Common
{
    public class CurrencyBar : Widget
    {
        [SerializeField]
        private CurrencyType m_currencyType;

        [SerializeField]
        private TextMeshProUGUI m_balanceText;

        [SerializeField]
        private Button m_button;

        [HideInEditorMode]
        [ShowInInspector]
        private float m_balance;

        public CurrencyType CurrencyType => m_currencyType;

        public UnityEvent<CurrencyType> Clicked { set; get; } = new();

        protected override void Awake()
        {
            base.Awake();

            // Whatever value was set in Editor, set it to 0
            SetBalanceAsync(0).Forget();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var game = GetService<Game>();
            Player player = game.Player;
            Currency currency = player.Currencies[CurrencyType.ToCurrencyCode()];
            currency.BalancedChanged += OnCurrencyBalanceChanged;
            m_button.onClick.AddListener(() => { Clicked?.Invoke(CurrencyType); });

            SetBalanceAsync(currency.Balance).Forget();
        }

        protected override void OnDisable()
        {
            var game = GetService<Game>();
            Player player = game.Player;
            Currency currency = player.Currencies[CurrencyType.ToCurrencyCode()];
            currency.BalancedChanged -= OnCurrencyBalanceChanged;
            m_button.onClick.RemoveAllListeners();

            base.OnDisable();
        }

        private void OnCurrencyBalanceChanged(object sender, CurrencyBalanceChanged e)
        {
            SetBalanceAsync(e.Balance, e.BalanceChange).Forget();
        }

        private async UniTaskVoid SetBalanceAsync(int newBalance, int balanceChange = 0)
        {
            if (balanceChange == 0)
            {
                m_balance = newBalance;
                m_balanceText.Set(Lexicon.Amount(newBalance));
                return;
            }

            int delta = Mathf.Abs(balanceChange);
            float animationSpeed = delta / UISettings.AddBalanceAnimationDuration;

            float endValue = UISettings.AddBalanceAnimationScale;
            float halfDuration = UISettings.AddBalanceAnimationDuration * 0.5f;
            m_balanceText.transform.DOScale(endValue, halfDuration)
                .OnComplete(() => m_balanceText.transform.DOScale(1.0f, halfDuration));

            if (balanceChange > 0)
            {
                while (m_balance < newBalance)
                {
                    m_balance += animationSpeed * Time.deltaTime;
                    if (m_balance > newBalance)
                    {
                        m_balance = newBalance;
                    }

                    m_balanceText.Set(Lexicon.Amount((int)m_balance));
                    await UniTask.Yield();
                }
            }
            else
            {
                while (m_balance > newBalance)
                {
                    m_balance -= animationSpeed * Time.deltaTime;
                    if (m_balance < newBalance)
                    {
                        m_balance = newBalance;
                    }

                    m_balanceText.Set(Lexicon.Amount((int)m_balance));
                    await UniTask.Yield();
                }
            }
        }

        public override void Show(bool animate = true)
        {
            base.Show(animate);

            var game = GetService<Game>();
            Player player = game.Player;
            Currency currency = player.Currencies[CurrencyType.ToCurrencyCode()];
            SetBalanceAsync(currency.Balance).Forget();
        }
    }
}
