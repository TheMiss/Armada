using Armageddon.Backend.Payloads;
using Armageddon.Localization;
using Armageddon.Maps;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Maps;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public enum InspectStageWindowResult
    {
        Back,
        Play
    }

    public class InspectStageWindow : Window
    {
        [Required]
        [SerializeField]
        private StageInfoWindow m_stageInfoWindow;

        [Required]
        [SerializeField]
        private StartingItemsWindow m_startingItemsWindow;

        public InspectStageWindowResult? WindowResult { private set; get; }

        public StageNode SelectedStageNode { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            UI.TabPageBottomBar.BackButton.onClick.AddListener(OnBackButtonClicked);
            UI.TabPageBottomBar.PlayButton.onClick.AddListener(OnPlayButtonClicked);
        }

        protected override void OnDisable()
        {
            UI.TabPageBottomBar.BackButton.onClick.RemoveListener(OnBackButtonClicked);
            UI.TabPageBottomBar.PlayButton.onClick.RemoveListener(OnPlayButtonClicked);

            base.OnDisable();
        }

        public async UniTask<InspectStageWindowResult?> InspectAsync(StageNode stageNode)
        {
            SelectedStageNode = stageNode;

            var player = GetService<Player>();
            Map map = player.Maps.Find(x => x.Id == stageNode.MapId);
            Stage stage = map.Stages.Find(x => x.Id == stageNode.StageId);

            m_stageInfoWindow.SetRewardBars(stage);
            m_startingItemsWindow.SetItems(player.PlayerInventory);

            WindowResult = null;

            await ShowDialogAsync(Transform.parent);

            return WindowResult;
        }

        private void OnBackButtonClicked()
        {
            DialogResult = false;
            WindowResult = InspectStageWindowResult.Back;
        }

        private void OnPlayButtonClicked()
        {
            var player = GetService<Player>();
            Map map = player.Maps.Find(x => x.Id == SelectedStageNode.MapId);
            Stage stage = map.Stages.Find(x => x.Id == SelectedStageNode.StageId);

            if (!player.CanStartGame(stage))
            {
                string title = Lexicon.InsufficientCurrency(CurrencyType.Energy);
                // TODO: Localize and make it show "You need 5 energy to play this stage."
                string text = "You need 5 energy to play this stage.";
                string ok = Texts.UI.OK;

                UI.AlertDialog.ShowInfoDialogAsync(title, text, ok).Forget();
                return;
            }

            DialogResult = true;
            WindowResult = InspectStageWindowResult.Play;
        }
    }
}
