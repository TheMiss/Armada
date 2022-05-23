using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.UI.InGameMenu;
using Armageddon.Worlds;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Armageddon.Games.Actions.BattleState
{
    public class UpdateBattle : BaseGameActionTask
    {
        public InGameMode InGameMode;

        protected override void OnExecute()
        {
            Game.UI.DisplayInGameMenu(true, InGameMode.Normal);
            Game.World.Commence(OnWorldFinished);

            SetPickingUIInEditor(false);

            // Game.UI.DisplayInGame(true, InGameMode);
            //
            // Game.HeroActor.CanTick = true;
            // Game.PlayerController.CanTick = true;
            // Game.UI.ScreenFader.FadeIn();
            //
            // if (InGameMode == InGameMode.Demo)
            // {
            //     Game.PlayerController.CanTick = false;
            // }
        }

        private void OnWorldFinished(WorldResult result)
        {
            Debug.Log("The battle is finished");

            SendEndGameAsync(result).Forget();
        }

        private async UniTaskVoid SendEndGameAsync(WorldResult result)
        {
            Game.UI.WaitForServerResponse.Show();

            var request = new EndGameRequest
            {
                StageId = result.StageId,
                KilledEnemies = result.KilledEnemies,
                ReceivedDrops = result.ReceivedDrops
            };

            EndGameReply reply = await Game.BackendDriver.EndGameAsync(request);

            string message = $"{reply.Level}, ";
            message += $"{reply.Exp}, ";
            // message += $"{reply.NextLevelExp}";

            Debug.Log(message);

            Game.UI.WaitForServerResponse.Hide();

            InGameResultWindowResult? windowResult = await Game.UI.InGameMenuScreen.ShowBattleResultAsync(reply);

            if (windowResult == InGameResultWindowResult.Continue)
            {
                await Game.UI.ScreenFader.FadeOutAsync();
                EndAction();
            }
        }

        // protected override async UniTaskVoid OnExecuteAsync()
        // {
        //     Game.UI.DisplayInGame(true, InGameMode.Normal);
        //     SetPicking(false);
        //     
        //     await Game.World.CommenceAsync();
        //     
        //     // EndAction();
        // }

        protected override void OnStop()
        {
            SetPickingUIInEditor(true);

            Game.World.ClearResources();
            Game.PlayerController.Stop();

            if (Game.IsPausing)
            {
                Game.Resume();
            }

            // Game.HeroActor.CanTick = false;
            // Game.PlayerController.CanTick = false;
        }
    }
}
