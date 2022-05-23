using Armageddon.Games.States;
using Cysharp.Threading.Tasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Armageddon.Games.Actions
{
    public class LoadDemoResources : BaseGameActionTask
    {
        [RequiredField]
        public BBParameter<GameState> GameState;

        protected override async UniTaskVoid OnExecuteAsync()
        {
            if (!(GameState.value is DemoResourcesLoadingState args))
            {
                Debug.LogError($"Args.value = {GameState.value}");
                return;
            }

            await UniTask.CompletedTask;

            EndAction();
        }
    }
}
