using Armageddon.UI.Base;
using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon.UI.Common
{
    public class FitToScreen : Widget
    {
        protected override void Awake()
        {
            base.Awake();

            RectTransform screenFaderRectTransform = UI.ScreenFader.RectTransform;
            Rect rect = screenFaderRectTransform.rect;
            Vector3 scale = UI.Transform.localScale;
            RectTransform.SetAnchor(AnchorPreset.MiddleCenter);
            // RectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            RectTransform.sizeDelta = new Vector2(rect.width, rect.height + 100);
        }
    }
}
