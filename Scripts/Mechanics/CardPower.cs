using Armageddon.AssetManagement;
using Armageddon.Sheets.Items;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Mechanics
{
    public class CardPower
    {
        public string Name => Sheet.Name;

        public CardSheet Sheet { get; private set; }

        public static async UniTask<CardPower> CreateAsync(int cardSheetId)
        {
            ItemSheet itemSheet = await Assets.LoadItemSheetAsync(cardSheetId);

            if (itemSheet is CardSheet cardSheet)
            {
                var cardPower = new CardPower
                {
                    Sheet = cardSheet
                };

                return cardPower;
            }

            Debug.LogError($"{itemSheet} is not {nameof(CardSheet)}");

            return null;
        }
    }
}
