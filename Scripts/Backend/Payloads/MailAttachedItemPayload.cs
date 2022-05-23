using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Mails;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public class MailAttachedItemPayload
    {
        public MailAttachedItemType Type;
        public ItemPayload Item;
        public CurrencyPayload Currency;
    }
}
