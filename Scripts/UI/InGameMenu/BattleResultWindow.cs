using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics;
using Armageddon.Sheets;
using Armageddon.UI.Base;
using Armageddon.UI.Common.OpenChestModule;
using Armageddon.UI.MainMenu.World.StartGame;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.InGameMenu
{
    public enum InGameResultWindowResult
    {
        Continue,
        World,
        Inventory
    }

    public class BattleResultWindow : Window
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private DropElement m_dropElementPrefab;

        [SerializeField]
        private TextMeshProUGUI m_playerNameText;

        [SerializeField]
        private TextMeshProUGUI m_playerLevelText;

        [SerializeField]
        private TextMeshProUGUI m_playerExpText;

        [SerializeField]
        private Image m_playerExpProgressBarFillerImage;

        [SerializeField]
        private List<StageInfoRewardBar> m_rewardBars;

        [SerializeField]
        private Button m_continueButton;

        [SerializeField]
        private RectTransform m_lootContentTransform;

        public InGameResultWindowResult? WindowResult { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_continueButton.onClick.AddListener(OnContinueClicked);
        }

        protected override void OnDisable()
        {
            m_continueButton.onClick.RemoveAllListeners();

            base.OnDisable();
        }

        private void OnContinueClicked()
        {
            DialogResult = true;
            WindowResult = InGameResultWindowResult.Continue;
        }

        private async UniTask SetExpAsync(EndGameReply reply)
        {
            ExpTable expTable = Assets.LoadExpTable();
            var player = GetService<Player>();
            int playerLevel = player.Level;
            int addingLevel = reply.Level - player.Level;
            double playerExp = player.Exp;

            if (!expTable.GetRow(playerLevel + 1, out ExpTableDetailsRow nextLevelRow))
            {
                return;
            }

            double targetExp = reply.Exp;
            float duration = 1.0f + addingLevel * 1.0f;
            long nextLevelExp = nextLevelRow.TotalExp;

            expTable.GetRow(playerLevel, out ExpTableDetailsRow currentLevelRow);

            m_playerLevelText.Set($"Level {playerLevel}");

            bool isDone = false;
            DOTween.To(() => playerExp, x => playerExp = x, targetExp, duration).OnUpdate(() =>
            {
                if (playerExp >= nextLevelRow.TotalExp)
                {
                    playerLevel++;

                    if (expTable.GetRow(playerLevel + 1, out nextLevelRow))
                    {
                        nextLevelExp = nextLevelRow.TotalExp;
                    }

                    expTable.GetRow(playerLevel, out currentLevelRow);

                    // TODO: Localize
                    m_playerLevelText.Set($"Level {playerLevel}");
                }

                double expDelta = playerExp - currentLevelRow.TotalExp;
                float percentage = (float)(expDelta / currentLevelRow.ExpForNextLevel);

                // TODO: Localize
                string text = $"EXP. {(long)playerExp} / {nextLevelExp}";
#if UNITY_EDITOR
                text += $" ({percentage:P})";
#endif
                m_playerExpText.Set(text);
                m_playerExpProgressBarFillerImage.fillAmount = percentage;
            }).OnComplete(() => { isDone = true; });

            while (!isDone)
            {
                await UniTask.Yield();
            }

            player.Level = playerLevel;
            player.Exp = (long)playerExp;
        }

        private async UniTask SetLoot(DropPayload[] dropObjects)
        {
            m_lootContentTransform.DestroyDesignRemnant();

            List<DropElement> dropElements = await DropElement.CreateManyAsync(dropObjects,
                m_dropElementPrefab, m_lootContentTransform);

            foreach (DropElement dropElement in dropElements)
            {
                dropElement.gameObject.SetActive(true);
                dropElement.Transform.PlayScaleAnimation();

                await UniTask.Delay(60);
            }
        }

        private async UniTask SetResultAsync(EndGameReply reply)
        {
            await SetLoot(reply.Drops);
            await SetExpAsync(reply);
        }

        public async UniTask<InGameResultWindowResult?> ShowResultAsync(EndGameReply reply)
        {
            SetResultAsync(reply).Forget();

            // AddExpAnimationAsync(reply).Forget();
            // SetLoot(reply.Drops).Forget();

            // // Required to play animation on Animator as Animator needs its gameObject to be active!
            Show();

            WindowResult = null;

            await ShowDialogAsync(Transform.parent);

            return WindowResult;
        }
    }
}
