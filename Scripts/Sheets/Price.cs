using System;
using System.Globalization;
using Armageddon.Backend.Payloads;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets
{
    [Serializable]
    public class Price
    {
        [TableColumnWidth(70)]
        [HorizontalGroup]
        [LabelWidth(100)]
        [SerializeField]
        private CurrencyType m_currencyType;

        [SerializeField]
        [HorizontalGroup]
        [LabelWidth(80)]
        private uint m_amount;

        public Price(CurrencyType currencyType, uint amount)
        {
            m_currencyType = currencyType;
            m_amount = amount;
        }

        [HideLabel]
        [HorizontalGroup]
        [DisplayAsString]
        [EnableGUI]
        [ShowInInspector]
        [LabelWidth(80)]
        private string FormattedAmount => m_amount.ToString("N0", new CultureInfo("en-US"));

        public CurrencyType CurrencyType => m_currencyType;

        public string Code => CurrencyType.ToCurrencyCode();

        public uint Amount => m_amount;
    }
}
