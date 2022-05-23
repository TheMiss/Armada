using Armageddon.Mechanics.Items;
using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class StartingItemTooltip : Tooltip
    {
        public void SetItem(Item item)
        {
            TitleText.Set(item.Name);

            string text = "+200 Blaster Damage\n";

            int number = Random.Range(0, 3);

            for (int i = 0; i < number; i++)
            {
                text += "+999 Attack Power";
            }

            DetailsText.Set(text);

            StartCoroutine(RefreshLayout(gameObject));
        }
    }
}
