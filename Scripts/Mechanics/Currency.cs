using System;
using Armageddon.Backend.Payloads;

namespace Armageddon.Mechanics
{
    public class CurrencyBalanceChanged : EventArgs
    {
        public CurrencyBalanceChanged(CurrencyType type, int balance, int balanceChange)
        {
            Type = type;
            Balance = balance;
            BalanceChange = balanceChange;
        }

        public CurrencyType Type { get; }
        public int Balance { get; }
        public int BalanceChange { get; }
    }

    public class Currency
    {
        private int m_balance;

        public Currency(CurrencyPayload payload)
        {
            Type = payload.Type;
            m_balance = payload.Amount;
        }

        public Currency(CurrencyType type, int balance)
        {
            Type = type;
            m_balance = balance;
        }

        public CurrencyType Type { get; private set; }

        public string Code => Type.ToCurrencyCode();

        /// <summary>
        ///     Can be used interchangeably with Amount.
        /// </summary>
        public int Balance => m_balance;

        /// <summary>
        ///     Can be used interchangeably with Balance.
        /// </summary>
        public int Amount => m_balance;

        public int AddBalance(int balanceChange)
        {
            m_balance += balanceChange;
            BalancedChanged?.Invoke(this, new CurrencyBalanceChanged(Type, m_balance, balanceChange));
            return m_balance;
        }

        public override string ToString()
        {
            return $"{Balance} {Type} ({Code})";
        }

        public event EventHandler<CurrencyBalanceChanged> BalancedChanged;
    }
}
