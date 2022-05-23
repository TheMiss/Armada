using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Mechanics;
using Cysharp.Threading.Tasks;

namespace Armageddon.Games.Actions
{
    // TODO: This is no longer used.
    public class LoadPlayerData : BaseGameActionTask
    {
        protected override async UniTaskVoid OnExecuteAsync()
        {
            Game.UI.WaitForServerResponse.Show();

            await Game.BackendDriver.DownloadFileAsync(Player.PlayerProfileFileName);
            await Game.BackendDriver.DownloadFileAsync(Player.PlayerInventoryFileName);
            LoadPlayerReply reply = await Game.BackendDriver.LoadPlayerAsync(new LoadPlayerRequest());

            var player = new Player();
            await player.ReinitializeAsync(reply);

            Game.Player = player;

            Game.UI.WaitForServerResponse.Hide();
            EndAction();
        }
    }
}
