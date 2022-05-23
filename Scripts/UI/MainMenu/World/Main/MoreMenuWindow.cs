using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.Main
{
    public class MoreMenuWindow : Window
    {
        [SerializeField]
        private Button m_accountButton;

        [SerializeField]
        private Button m_mailboxButton;

        [SerializeField]
        private Button m_settingsButton;

        [SerializeField]
        private Button m_supportButton;

        protected override void Awake()
        {
            base.Awake();

            m_accountButton.onClick.AddListener(() => OnButtonClicked(m_accountButton));
            m_mailboxButton.onClick.AddListener(() => OnButtonClicked(m_mailboxButton));
            m_settingsButton.onClick.AddListener(() => OnButtonClicked(m_settingsButton));
            m_supportButton.onClick.AddListener(() => OnButtonClicked(m_supportButton));
        }

        private void OnButtonClicked(Button button)
        {
            if (button == m_accountButton)
            {
                CloseDialog();
                UI.AccountWindow.BlockerColor = Color.black;
                UI.AccountWindow.ShowDialogAsync().Forget();
            }
            else if (button == m_mailboxButton)
            {
                CloseDialog();
                UI.MailboxWindow.ShowDialogAsync().Forget();
                UI.TopBar.Transform.SetAsLastSibling();
            }
            else if (button == m_settingsButton)
            {
                CloseDialog();
                UI.SettingsWindow.ShowDialogAsync().Forget();
            }

            DialogResult = true;
        }
    }
}
