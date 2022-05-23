using Armageddon.Configuration;
using Armageddon.UI.Base;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Armageddon.UI.MainMenu
{
    public class MainMenuBar : Widget
    {
        private Vector2 m_endAnchoredPosition;
        private Vector2 m_initialAnchoredPosition;

        private TweenerCore<float, float, FloatOptions> m_tweener;

        protected override void Awake()
        {
            base.Awake();

            m_initialAnchoredPosition = RectTransform.anchoredPosition;

            Rect rect = RectTransform.rect;
            Vector2 endAnchoredPosition = m_initialAnchoredPosition;
            endAnchoredPosition.y -= rect.height + 10;

            m_endAnchoredPosition = endAnchoredPosition;

            RegisterService(this);
        }

        public override void Show(bool animate = true)
        {
            if (m_tweener != null)
            {
                m_tweener.Kill();
                m_tweener = null;
                DoTweenAnimation = null;
            }

            DoTweenAnimation = () =>
            {
                m_tweener = DOTween.To(() => RectTransform.anchoredPosition.y,
                    y => RectTransform.anchoredPosition = new Vector2(m_initialAnchoredPosition.x, y),
                    m_initialAnchoredPosition.y, UISettings.TopBarAnimationDuration);

                m_tweener.OnComplete(OnOpenAnimationFinished);
                m_tweener.OnKill(() => m_tweener = null);
            };

            base.Show(animate);
        }

        public override void Hide(bool animate = true)
        {
            if (m_tweener != null)
            {
                m_tweener.Kill();
                m_tweener = null;
                DoTweenAnimation = null;
            }

            DoTweenAnimation = () =>
            {
                m_tweener = DOTween.To(() => RectTransform.anchoredPosition.y,
                    y => RectTransform.anchoredPosition = new Vector2(m_endAnchoredPosition.x, y),
                    m_endAnchoredPosition.y, UISettings.TopBarAnimationDuration);

                m_tweener.OnComplete(OnCloseAnimationFinished);
                m_tweener.OnKill(() => m_tweener = null);
            };

            base.Hide(animate);
        }
    }
}
