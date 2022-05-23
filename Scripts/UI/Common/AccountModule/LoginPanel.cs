using Armageddon.Externals.OdinInspector;
using Armageddon.Localization;
using Armageddon.UI.Base;
using Armageddon.UI.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Common.AccountModule
{
    public class LoginPanel : Panel
    {
        [SerializeField]
        private Button m_playAsGuest;

        [SerializeField]
        private Button m_facebookButton;

        [SerializeField]
        private Button m_googleButton;

        [SerializeField]
        private Button m_appleButton;

        [SerializeField]
        private Button m_emailButton;

        [SerializeField]
        private GameObject m_firstTimeObject;

        [SerializeField]
        private GameObject m_registerAccountObject;

        [SerializeField]
        private GameObject m_signedOutObject;

        public Button PlayAsGuest => m_playAsGuest;
        public Button FacebookButton => m_facebookButton;
        public Button GoogleButton => m_googleButton;
        public Button AppleButton => m_appleButton;
        public Button EmailButton => m_emailButton;

        public void DisableAllButtons()
        {
            PlayAsGuest.gameObject.SetActive(false);
            FacebookButton.gameObject.SetActive(false);
            GoogleButton.gameObject.SetActive(false);
            AppleButton.gameObject.SetActive(false);
            EmailButton.gameObject.SetActive(false);
        }

        public void AddOnClickButtonsListener(UnityAction<Button> callback)
        {
            m_playAsGuest.onClick.AddListener(() => callback(m_playAsGuest));
            m_facebookButton.onClick.AddListener(() => callback(m_facebookButton));
            m_googleButton.onClick.AddListener(() => callback(m_googleButton));
            m_appleButton.onClick.AddListener(() => callback(m_appleButton));
            m_emailButton.onClick.AddListener(() => callback(m_emailButton));
        }

        [Button]
        [GUIColorDefaultButton]
        public void SetFirstTime()
        {
            m_playAsGuest.gameObject.SetActive(true);
            m_facebookButton.gameObject.SetActive(true);
            m_googleButton.gameObject.SetActive(true);
            m_appleButton.gameObject.SetActive(true);
            m_emailButton.gameObject.SetActive(true);

            m_firstTimeObject.SetActive(true);
            m_registerAccountObject.SetActive(false);
            m_signedOutObject.SetActive(false);

            SetMode(ButtonMode.SignIn);
        }

        [Button]
        [GUIColorDefaultButton]
        public void SetRegisterAccount()
        {
            m_playAsGuest.gameObject.SetActive(false);
            m_facebookButton.gameObject.SetActive(true);
            m_googleButton.gameObject.SetActive(true);
            m_appleButton.gameObject.SetActive(true);
            m_emailButton.gameObject.SetActive(true);

            m_firstTimeObject.SetActive(false);
            m_registerAccountObject.SetActive(true);
            m_signedOutObject.SetActive(false);

            SetMode(ButtonMode.SignUp);
        }

        [Button]
        [GUIColorDefaultButton]
        public void SetAfterSignOut()
        {
            m_playAsGuest.gameObject.SetActive(false);
            m_facebookButton.gameObject.SetActive(true);
            m_googleButton.gameObject.SetActive(true);
            m_appleButton.gameObject.SetActive(true);
            m_emailButton.gameObject.SetActive(true);

            m_firstTimeObject.SetActive(false);
            m_registerAccountObject.SetActive(false);
            m_signedOutObject.SetActive(true);

            SetMode(ButtonMode.SignIn);
        }

        private void SetMode(ButtonMode mode)
        {
            SetButton(m_facebookButton, mode, "Facebook");
            SetButton(m_googleButton, mode, "Google");
            SetButton(m_appleButton, mode, "Apple");
            SetButton(m_emailButton, mode, "Email");

#if UNITY_ANDROID && !UNITY_EDITOR
            m_appleButton.gameObject.SetActive(false);
#endif
        }

        private static void SetButton(Button button, ButtonMode mode, string type)
        {
            string modeText = mode == ButtonMode.SignIn ? Texts.UI.SignInWith : Texts.UI.SignUpWith;
            string text = $"{modeText} {type}";
            button.SetText(text);
        }

        private enum ButtonMode
        {
            SignIn,
            SignUp
        }
    }
}
