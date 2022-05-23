using System;
using Armageddon.Worlds;
using DG.Tweening;
using UnityEngine;

namespace Armageddon.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void PlayScaleAnimation(this Transform transform, float scalePercent = 0.15f,
            Action completeCallback = null)
        {
            float originalScale = transform.localScale.x;
            float startScale = originalScale * World.ScaleAnimationStartPercent;
            float endValueStep1 = originalScale + originalScale * scalePercent;
            float halfDuration = World.ScaleAnimationDuration * 0.5f;
            transform.localScale = new Vector3(startScale, startScale, startScale);
            transform.DOScale(endValueStep1, halfDuration).OnComplete(
                () => transform.DOScale(originalScale, halfDuration).OnComplete(
                    () => { completeCallback?.Invoke(); }));
        }

        // public static void SetLayer(this MonoBehaviour gameObject, LayerMask layerMask, bool applyToChildren = true)
        // {
        //     gameObject.layer = layerMask;
        //     foreach (var VARIABLE in gameObject)
        //     {
        //         
        //     }
        // }
    }
}
