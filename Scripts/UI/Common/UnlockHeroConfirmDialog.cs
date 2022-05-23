using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Characters;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;

namespace Armageddon.UI.Common
{
    public class UnlockHeroConfirmDialog : AlertDialog
    {
        public async UniTask<bool?> ShowUnlockAsync(Hero hero, CurrencyType currencyType)
        {
            // Do display the icon here
            // SetHeroIcon(hero);

            string code = currencyType.ToCurrencyCode();

            TitleText.Set("Attention!");
            MessageText.Set($"Are you sure you want to unlock {hero.Sheet.Name} for " +
                $"<sprite name=\"{currencyType.ToHumanReadable()}\">{hero.Prices[code]}");
            AcceptButtonText.Set("Yes");
            RejectButtonText.Set("Later");

            transform.SetAsLastSibling();

            return await ShowDialogAsync();
        }
    }
}
