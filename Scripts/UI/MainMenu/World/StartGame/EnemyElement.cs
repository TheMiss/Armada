using Armageddon.Sheets.Actors;
using Armageddon.UI.Base;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class EnemyElement : SelectableElement<EnemyElement>
    {
        public EnemySheet EnemySheet { get; private set; }
        public override object Object => EnemySheet;

        public void Initialize(EnemySheet enemySheet)
        {
            EnemySheet = enemySheet;
        }
    }
}
