using System.Threading;
using Armageddon.Configuration;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Armageddon
{
    public static class TweenUtility
    {
        public static async UniTask<int> ChangeValue(int initialValue, int newValue, TextMeshProUGUI text,
            string textFormat, CancellationToken token)
        {
            float currentValue = initialValue;

            if (initialValue == newValue)
            {
                text.text = string.Format(textFormat, initialValue);
                return initialValue;
            }

            int valueChange = newValue - initialValue;
            int delta = Mathf.Abs(valueChange);
            float animationSpeed = delta / UISettings.AddBalanceAnimationDuration;
            float endValue = UISettings.AddBalanceAnimationScale;
            float halfDuration = UISettings.AddBalanceAnimationDuration * 0.5f;

            text.transform.DOScale(endValue, halfDuration).OnComplete(() => text.transform.DOScale(1.0f, halfDuration));

            //DoScaleText(text, endValue, 1.0f, halfDuration, token).Forget();

            if (valueChange > 0)
            {
                while (currentValue < newValue)
                {
                    currentValue += animationSpeed * Time.deltaTime;
                    if (currentValue > newValue)
                    {
                        currentValue = newValue;
                    }

                    text.text = string.Format(textFormat, (int)currentValue);
                    await UniTask.Yield(token);
                }
            }
            else
            {
                while (currentValue > newValue)
                {
                    currentValue -= animationSpeed * Time.deltaTime;
                    if (currentValue < newValue)
                    {
                        currentValue = newValue;
                    }

                    text.text = string.Format(textFormat, (int)currentValue);
                    await UniTask.Yield(token);
                }
            }

            return (int)currentValue;
        }

        // private static async UniTaskVoid DoScaleText(TextMeshProUGUI text, float stepOneScale, float stepTwoScale,
        //     float duration, CancellationToken token)
        // {
        //     await text.transform.DOScale(stepOneScale, duration).WithCancellation(token);
        //     await text.transform.DOScale(stepTwoScale, duration).WithCancellation(token);
        // }
    }
}
