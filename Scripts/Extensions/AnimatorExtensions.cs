using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Extensions
{
    public static class AnimatorExtensions
    {
        public static async UniTask PlayAsync(this Animator animator, MonoBehaviour coroutineRunner, string stateName,
            int layer = 0, float normalizedTime = 0)
        {
            animator.Play(stateName, layer, normalizedTime);

            await NotifyAnimationFinishDelayed(animator, stateName).ToUniTask(coroutineRunner);
        }

        public static IEnumerator NotifyAnimationFinishDelayed(Animator animator, string stateName,
            Action finishedCallback = null)
        {
            bool isDone = false;

            while (!isDone)
            {
                // if (!animator.IsInTransition(0))
                {
                    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

                    int stateNameHash = Animator.StringToHash(stateName);

                    if (animatorStateInfo.shortNameHash == stateNameHash &&
                        animatorStateInfo.normalizedTime >= 1)
                    {
                        isDone = true;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            finishedCallback?.Invoke();
        }
    }
}
