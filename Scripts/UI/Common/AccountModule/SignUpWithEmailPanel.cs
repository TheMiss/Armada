using Armageddon.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Common.AccountModule
{
    public class SignUpWithEmailPanel : Panel
    {
        [SerializeField]
        private TMP_InputField m_emailInputField;

        [SerializeField]
        private TMP_InputField m_passwordInputField;

        [SerializeField]
        private TMP_InputField m_confirmPasswordInputField;

        [SerializeField]
        private Toggle m_showPasswordToggle;

        [SerializeField]
        private Button m_signUpButton;

        public TMP_InputField EmailInputField => m_emailInputField;
        public TMP_InputField PasswordInputField => m_passwordInputField;
        public TMP_InputField ConfirmPasswordInputField => m_confirmPasswordInputField;
        public Toggle ShowPasswordToggle => m_showPasswordToggle;
        public Button SignUpButton => m_signUpButton;

        public void AddOnClickButtonsListener(UnityAction<Button> callback)
        {
            m_signUpButton.onClick.AddListener(() => callback(m_signUpButton));
        }

        public void AddOnValueChangedTogglesListener(UnityAction<Toggle> callback)
        {
            m_showPasswordToggle.onValueChanged.AddListener(value =>
                callback(m_showPasswordToggle));
        }
    }
}
