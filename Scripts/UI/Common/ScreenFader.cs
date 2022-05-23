using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common
{
    public enum ScreenFaderState
    {
        FadingIn,
        FadedIn,
        FadingOut,
        FadedOut
    }

    public class ScreenFader : Widget
    {
        [SerializeField]
        private Image m_backgroundImage;

        [SerializeField]
        private float m_fadeInDuration = 1.0f;

        [SerializeField]
        private float m_fadeOutDuration = 1.0f;

        public ScreenFaderState State { private set; get; }

        protected override void Awake()
        {
            base.Awake();
            CanTick = true;
        }

        public async UniTask<ScreenFaderState> FadeInAsync()
        {
            gameObject.SetActive(true);
            m_backgroundImage.gameObject.SetActive(true);
            m_backgroundImage.canvasRenderer.SetAlpha(1.0f);
            m_backgroundImage.CrossFadeAlpha(0.0f, m_fadeInDuration, true);

            State = ScreenFaderState.FadingIn;

            while (State == ScreenFaderState.FadingIn)
            {
                float alpha = m_backgroundImage.canvasRenderer.GetAlpha();
                if (alpha <= 0)
                {
                    State = ScreenFaderState.FadedIn;
                    break;
                }

                await UniTask.Yield();
            }

            m_backgroundImage.gameObject.SetActive(false);

            return State;
        }

        public async UniTask<ScreenFaderState> FadeOutAsync()
        {
            gameObject.SetActive(true);
            m_backgroundImage.gameObject.SetActive(true);
            m_backgroundImage.canvasRenderer.SetAlpha(0.0f);
            m_backgroundImage.CrossFadeAlpha(1.0f, m_fadeOutDuration, true);

            State = ScreenFaderState.FadingOut;

            while (State == ScreenFaderState.FadingOut)
            {
                float alpha = m_backgroundImage.canvasRenderer.GetAlpha();
                if (alpha >= 1)
                {
                    State = ScreenFaderState.FadedOut;
                    break;
                }

                await UniTask.Yield();
            }

            // Must not call this otherwise, the black screen will disappear.
            //m_backgroundImage.gameObject.SetActive(false);

            return State;
        }

        public void FadeIn()
        {
            FadeInAsync().Forget();
        }

        public void FadeOut()
        {
            FadeOutAsync().Forget();
        }
    }
}
