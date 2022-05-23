using System;
using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Maps;
using Armageddon.Sheets.Items;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class StageInfoRewardBar : Widget
    {
        [SerializeField]
        private Toggle m_starToggle;

        [SerializeField]
        private TextMeshProUGUI m_leftText;

        [SerializeField]
        private TextMeshProUGUI m_rightText;

        [SerializeField]
        private Image m_rewardIcon;

        public Toggle StarToggle => m_starToggle;

        public async UniTaskVoid SetRewardAsync(RewardPayload reward)
        {
            Sprite sprite = null;

            switch (reward.Type)
            {
                case RewardType.GoldShard:
                    sprite = UI.SpriteBank.GoldShard;
                    break;
                case RewardType.RedGem:
                    sprite = UI.SpriteBank.RedGem;
                    break;
                case RewardType.Item:
                {
                    ItemSheet itemSheet = await Assets.LoadItemSheetAsync(reward.ItemSheetId);
                    sprite = itemSheet.Icon;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_rewardIcon.sprite = sprite;
            m_rightText.Set($"{reward.Quantity}");
        }
    }

    public static class StageInfoRewardBarExtensions
    {
        public static void SetData(this List<StageInfoRewardBar> rewardBars, Stage stage)
        {
            for (int i = 0; i < rewardBars.Count; i++)
            {
                StageInfoRewardBar rewardBar = rewardBars[i];
                ScoreRequirementPayload scoreRequirement = stage.Payload.ScoreRequirements[i];
                RewardPayload reward = stage.Payload.Rewards[i];

                rewardBar.SetRewardAsync(reward).Forget();
                rewardBar.StarToggle.isOn = i < stage.Payload.StarCount;
            }
        }
    }
}
