using Armageddon.Sheets.Items;
using Purity.Common.Extensions;

namespace Armageddon.UI.MainMenu.Upgrades.Powers
{
    public class PowerTooltip : Tooltip
    {
        public void SetPower(CardSheet cardSheet)
        {
            TitleText.Set(cardSheet.Name);

            // TODO: Show multiple effects.
            string detailsText =
                $"{cardSheet.Effects[0].Description}";


            DetailsText.Set(detailsText);

            StartCoroutine(RefreshLayout(gameObject));
        }
    }
}
