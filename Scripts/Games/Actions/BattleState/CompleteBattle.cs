using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Games.Actions.BattleState
{
    public class CompleteBattle : BaseGameActionTask
    {
        protected override async UniTaskVoid OnExecuteAsync()
        {
            Debug.Log("Hey");
            await UniTask.Delay(1000);
        }
    }
}
