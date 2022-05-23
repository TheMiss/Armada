using Cysharp.Threading.Tasks;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Armageddon.Games.Actions
{
    [Category("Games")]
    public abstract class BaseGameActionTask : ActionTask<Game>
    {
        protected Game Game { private set; get; }

        /// <summary>
        ///     Use for initialization. This is called only once in the lifetime of the task.
        ///     Return null if init was successful. Return an error string otherwise
        /// </summary>
        protected override string OnInit()
        {
            // Not to pollute console log
            // Debug.Log($"{GetType().Name}.OnInit");
            Game = agent.GetComponent<Game>();

            return null;
        }

        /// <summary>
        ///     This is called once each time the task is enabled.
        ///     Call EndAction() to mark the action as finished, either in success or failure.
        ///     EndAction can be called from anywhere.
        /// </summary>
        protected override void OnExecute()
        {
            Debug.Log($"{GetType().Name}.OnExecute");

            OnExecuteAsync().Forget();
        }

        protected virtual async UniTaskVoid OnExecuteAsync()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        ///     Called once per frame while the action is active.
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        ///     Called when the task is disabled.
        /// </summary>
        protected override void OnStop()
        {
            Debug.Log($"{GetType().Name}.OnStop");
        }

        /// <summary>
        ///     Called when the task is paused.
        /// </summary>
        protected override void OnPause()
        {
            Debug.Log($"{GetType().Name}.OnPause");
        }

        /// <summary>
        ///     Very convenient for debugging especially for UI the has a big bounds to prevent selecting game objects...
        /// </summary>
        protected void SetPickingUIInEditor(bool enabled)
        {
#if UNITY_EDITOR
            if (enabled)
            {
                SceneVisibilityManager.instance.EnablePicking(Game.UI.gameObject, true);
            }
            else
            {
                SceneVisibilityManager.instance.DisablePicking(Game.UI.gameObject, true);
            }
#endif
        }
    }
}
