using Armageddon.Games.States;
using Cysharp.Threading.Tasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Armageddon.Games.Actions
{
    public class LoadBattleResources : BaseGameActionTask
    {
        [RequiredField]
        public BBParameter<GameState> State;

        protected override async UniTaskVoid OnExecuteAsync()
        {
            if (!(State.value is BattleResourcesLoadingState state))
            {
                Debug.LogError($"state.value = {State.value}");
                return;
            }

            Game.UI.ClearMainMenuResources();

            await Game.World.LoadObjects(state.StartGameReply, Game.Player);

            Game.World.Prepare();
            Game.UI.ScreenFader.FadeIn();

            EndAction();
        }
    }
}
