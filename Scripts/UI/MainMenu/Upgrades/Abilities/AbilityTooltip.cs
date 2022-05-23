using Armageddon.Localization;
using Armageddon.Mechanics.Abilities;
using Purity.Common.Extensions;

namespace Armageddon.UI.MainMenu.Upgrades.Abilities
{
    public class AbilityTooltip : Tooltip
    {
        public void SetAbility(PlayerAbility playerAbility)
        {
            TitleText.Set(playerAbility.Sheet.Name);

            // TODO: Show multiple effects.
            string detailsText = string.Empty;

            if (playerAbility.Level > 0)
            {
                string currentLevelText = Texts.Message.CurrentLevel;
                detailsText +=
                    $"<b>{currentLevelText}: {playerAbility.Level}</b>\n" +
                    $"{playerAbility.Effects[0].Description}\n\n";
            }

            // TODO: Check If there is next level
            {
                string nextLevelText = playerAbility.Level == 0 ? Texts.Message.FirstLevel : Texts.Message.NextLevel;
                detailsText +=
                    $"<b>{nextLevelText}</b>\n" +
                    $"{playerAbility.NextLevelEffects[0].Description}";
            }

            DetailsText.Set(detailsText);

            StartCoroutine(RefreshLayout(gameObject));
        }
    }
}
