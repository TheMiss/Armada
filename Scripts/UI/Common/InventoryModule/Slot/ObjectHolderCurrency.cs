using Armageddon.Localization;
using Armageddon.Mechanics;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class ObjectHolderCurrency : ObjectHolder
    {
        [SerializeField]
        private TextMeshProUGUI m_amountText;

        public void SetCurrency(Currency currency)
        {
            Sprite icon = UI.SpriteBank.GetCurrencyIcon(currency.Type);
            SetIcon(icon);

            m_amountText.Set(Lexicon.Amount(currency.Amount));
        }
    }
}
