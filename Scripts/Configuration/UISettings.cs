using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Configuration
{
    // Suppress suggestion as we usually use OdinInspector to tweak these values.
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    public static class UISettings
    {
        public static readonly float TopBarAnimationDuration = 0.5f;
        public static readonly float AddBalanceAnimationDuration = 0.5f;
        public static readonly float AddBalanceAnimationScale = 1.15f;
        public static readonly bool InstantSwapItem = true;
        public static readonly Color BlockerColor = new(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);
        public static readonly float DimSlotAlpha = 0.0f;
        public static readonly float ShowPanelDuration = 0.5f;
        public static readonly float ScrollSnapDuration = 0.5f;
        public static readonly bool UpdateMailDataTimeInRealTime = true;

        [OnValueChanged(nameof(OnValueChanged))]
        public static readonly bool HideInsteadDimSlot = false;

        public static void OnValueChanged()
        {
            Debug.Log($"DimSlotAlpha = {DimSlotAlpha}");
        }
    }
}
