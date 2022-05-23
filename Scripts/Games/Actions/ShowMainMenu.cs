using Armageddon.Games.States;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Armageddon.Games.Actions
{
    public class ShowMainMenu : BaseGameActionTask
    {
        [RequiredField]
        public BBParameter<GameState> StateData;

        protected override void OnExecute()
        {
            Game.UI.DisplayMainMenu().Forget();

            if (StateData.value is MainMenuState args)
            {
                if (args.BackFromDemo)
                {
                    //Game.UI.LoadoutScreen.Show();
                }
            }
        }
    }
}
