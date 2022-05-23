#define ENABLE_EMAIL_SIGN_IN

using System;
using System.Net.Mail;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.AccountModule
{
    public enum AccountWindowMode
    {
        FirstTime,
        RegisterAccount,
        AfterLogOut,
        LoggedIn
    }

    public partial class AccountWindow : Window
    {
        public enum SignInType
        {
            Email,
            Facebook,
            Google,
            Apple
        }

        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private Button m_closeButton;

        [SerializeField]
        private TextMeshProUGUI m_playerIdText;

        [SerializeField]
        private RectTransform m_contentTransform;

        [SerializeField]
        private LoginPanel m_loginPanel;

        [SerializeField]
        private SignInWithEmailPanel m_signInWithEmailPanel;

        [SerializeField]
        private SignUpWithEmailPanel m_signUpWithEmailPanel;

        [SerializeField]
        private SignedInPanel m_signedInPanel;

        [ShowInPlayMode]
        private Panel m_currentPanel;

        protected string Status { get; set; }

        protected Texture2D LastResponseTexture { get; set; }

        protected string LastResponse { get; set; }

        private BackendDriver BackendDriver => UI.Game.BackendDriver;

        protected override void Awake()
        {
            base.Awake();

            m_backButton.onClick.AddListener(() => OnButtonClicked(m_backButton));
            m_closeButton.onClick.AddListener(() => OnButtonClicked(m_closeButton));

            m_loginPanel.DisableAllButtons();
            m_loginPanel.AddOnClickButtonsListener(OnLoginPanelButtonClicked);
#if ENABLE_FACEBOOK_SIGN_IN
            m_loginPanel.FacebookButton.gameObject.SetActive(true);
#endif
#if ENABLE_GOOGLE_SIGN_IN
            m_loginPanel.GoogleButton.gameObject.SetActive(true);
#endif
#if ENABLE_EMAIL_SIGN_IN
            m_loginPanel.EmailButton.gameObject.SetActive(true);
#endif

            m_signInWithEmailPanel.AddOnClickButtonsListener(OnSignInWithEmailPanelButtonClicked);
            m_signInWithEmailPanel.AddOnValueChangedTogglesListener(OnSignInWithEmailPanelToggleChanged);

            m_signUpWithEmailPanel.AddOnClickButtonsListener(OnSignUpPanelButtonClicked);
            m_signUpWithEmailPanel.AddOnValueChangedTogglesListener(OnSignUpPanelToggleChanged);

            m_signedInPanel.AddOnClickButtonsListener(OnSignedInPanelButtonClicked);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_backButton.gameObject.SetActive(false);
            m_loginPanel.gameObject.SetActive(false);
            m_signInWithEmailPanel.gameObject.SetActive(false);
            m_signUpWithEmailPanel.gameObject.SetActive(false);

            Panel showingPanel;
            AuthenticationType authenticationType = BackendDriver.AuthenticationType;

            if (authenticationType == AuthenticationType.RememberMe ||
                authenticationType == AuthenticationType.EmailAndPassword ||
                authenticationType == AuthenticationType.Facebook ||
                authenticationType == AuthenticationType.Google ||
                authenticationType == AuthenticationType.Apple)
            {
                showingPanel = m_signedInPanel;
            }
            else
            {
                showingPanel = m_loginPanel;
            }

            ShowPanel(showingPanel);

            UpdateAccountId();
        }

        private void OnButtonClicked(Button button)
        {
            if (button == m_closeButton)
            {
                DialogResult = false;
            }
            else if (button == m_backButton)
            {
                if (m_currentPanel != m_loginPanel)
                {
                    ShowPanel(m_loginPanel);
                }
            }
        }

        private void OnLoginPanelButtonClicked(Button button)
        {
            if (button == m_loginPanel.PlayAsGuest)
            {
                SignInAsGuestAsync().Forget();
            }
            else if (button == m_loginPanel.FacebookButton)
            {
#if ENABLE_FACEBOOK_SIGN_IN

                async UniTask Async()
                {
                    await SignInWithFacebookAsync(true);
                    await Game.LoadPlayerDataAsync();
                }

                Async().Forget();
#endif
            }
            else if (button == m_loginPanel.GoogleButton)
            {
#if ENABLE_GOOGLE_SIGN_IN

                async UniTask Async()
                {
                    await SignInWithGoggleAsync(true);
                    await Game.LoadPlayerDataAsync();
                }

                Async().Forget();
#endif
            }
            else if (button == m_loginPanel.EmailButton)
            {
                ShowPanel(m_signInWithEmailPanel);
            }
        }

        private async UniTask SignInAsGuestAsync()
        {
            UI.WaitForServerResponse.Show();

            SignInReply reply = await BackendDriver.SignInWithDeviceAsync();

            UI.WaitForServerResponse.Hide();

            if (UI.Game.ValidateReply(reply))
            {
                UI.Game.IsFirstTime = false;
                await UI.Game.LoadPlayerDataAsync();
                DialogResult = true;
            }
        }

        private async UniTask SignOutAsync()
        {
            UI.WaitForServerResponse.Show();

            await BackendDriver.SignOut();

            ShowPanel(m_loginPanel);
            SetProfileImage(null);
            SetProfileName(string.Empty);
            // SceneManager.LoadScene("Main");

            UI.WaitForServerResponse.Hide();
        }

        private void OnSignUpPanelToggleChanged(Toggle toggle)
        {
            if (toggle == m_signUpWithEmailPanel.ShowPasswordToggle)
            {
                m_signUpWithEmailPanel.PasswordInputField.contentType = toggle.isOn
                    ? TMP_InputField.ContentType.Standard
                    : TMP_InputField.ContentType.Password;
                m_signUpWithEmailPanel.ConfirmPasswordInputField.contentType = toggle.isOn
                    ? TMP_InputField.ContentType.Standard
                    : TMP_InputField.ContentType.Password;

                m_signUpWithEmailPanel.PasswordInputField.ForceLabelUpdate();
                m_signUpWithEmailPanel.ConfirmPasswordInputField.ForceLabelUpdate();
            }
        }

        private void OnSignUpPanelButtonClicked(Button button)
        {
            if (button == m_signUpWithEmailPanel.SignUpButton)
            {
                RegisterAccountAndPasswordAsync().Forget();
            }
        }

        private bool IsValidEmailFormat(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);

                return true;
            }
            catch (FormatException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        private void OnSignInWithEmailPanelButtonClicked(Button button)
        {
            if (button == m_signInWithEmailPanel.LogInButton)
            {
                async UniTask Async()
                {
                    await SignInWithEmailAsync();
                    await Game.LoadPlayerDataAsync();
                }

                Async().Forget();
            }
            else if (button == m_signInWithEmailPanel.SignUpWithEmailButton)
            {
                ShowPanel(m_signUpWithEmailPanel);
            }
        }

        private void OnSignInWithEmailPanelToggleChanged(Toggle toggle)
        {
            if (toggle == m_signInWithEmailPanel.ShowPasswordToggle)
            {
                m_signInWithEmailPanel.PasswordInputField.contentType = toggle.isOn
                    ? TMP_InputField.ContentType.Standard
                    : TMP_InputField.ContentType.Password;

                m_signInWithEmailPanel.PasswordInputField.ForceLabelUpdate();
            }
        }

        private void OnSignedInPanelButtonClicked(Button button)
        {
            if (button == m_signedInPanel.LogOutButton)
            {
                SignOutAsync().Forget();
            }
        }

        private void ShowPanel(Panel panel)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (m_currentPanel != null)
            {
                m_currentPanel.IsActive = false;
            }

            m_currentPanel = panel;
            m_currentPanel.Show();

            if (panel == m_loginPanel ||
                panel == m_signedInPanel)
            {
                if (BackendDriver.AuthenticationType == AuthenticationType.None)
                {
                    m_backButton.gameObject.SetActive(false);
                    m_closeButton.gameObject.SetActive(false);
                }
                else
                {
                    m_backButton.gameObject.SetActive(false);
                    m_closeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                m_backButton.gameObject.SetActive(true);
                m_closeButton.gameObject.SetActive(false);
            }

            if (panel == m_loginPanel)
            {
                switch (BackendDriver.IsAuthenticated)
                {
                    case false when UI.Game.IsFirstTime:
                        m_loginPanel.SetFirstTime();
                        break;
                    case false:
                        m_loginPanel.SetAfterSignOut();
                        break;
                    case true when BackendDriver.AuthenticationType == AuthenticationType.Device:
                        m_loginPanel.SetRegisterAccount();
                        break;
                }
            }

            UpdateAccountId();
        }

        private void UpdateAccountId()
        {
            if (BackendDriver.IsAuthenticated)
            {
                m_playerIdText.Set($"ID:{BackendDriver.PlayerId.Green()}");
            }
            else
            {
                m_playerIdText.Set(string.Empty);
            }
        }

        public void SetEmail(string email)
        {
            m_signedInPanel.SetProfileText(email);
        }

        private void SetProfileImage(Sprite sprite)
        {
            m_signedInPanel.ProfileImage.sprite = sprite;
        }

        private void SetProfileName(string profileName)
        {
            m_signedInPanel.SetProfileText(profileName);
        }

        private void ShowWaitForServerResponse(bool show)
        {
            if (show)
            {
                UI.WaitForServerResponse.Show();
            }
        }

        private void HideWaitForServerResponse(bool hide)
        {
            if (hide)
            {
                UI.WaitForServerResponse.Hide();
            }
        }
    }
}
