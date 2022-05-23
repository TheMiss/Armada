using Armageddon.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Common.AccountModule
{
    public class SignInWithEmailPanel : Panel
    {
        [SerializeField]
        private TMP_InputField m_emailInputField;

        [SerializeField]
        private TMP_InputField m_passwordInputField;

        [SerializeField]
        private Toggle m_showPasswordToggle;

        [SerializeField]
        private Button m_logInButton;

        [SerializeField]
        private Button m_signUpWithEmailButton;

        public TMP_InputField EmailInputField => m_emailInputField;
        public TMP_InputField PasswordInputField => m_passwordInputField;
        public Toggle ShowPasswordToggle => m_showPasswordToggle;
        public Button LogInButton => m_logInButton;

        public Button SignUpWithEmailButton => m_signUpWithEmailButton;

        public void AddOnClickButtonsListener(UnityAction<Button> callback)
        {
            m_logInButton.onClick.AddListener(() => callback(m_logInButton));
            m_signUpWithEmailButton.onClick.AddListener(() => callback(m_signUpWithEmailButton));
        }

        public void AddOnValueChangedTogglesListener(UnityAction<Toggle> callback)
        {
            m_showPasswordToggle.onValueChanged.AddListener(value =>
                callback(m_showPasswordToggle));
        }
    }
}
