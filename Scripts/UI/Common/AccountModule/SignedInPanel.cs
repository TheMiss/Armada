using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Common.AccountModule
{
    public class SignedInPanel : Panel
    {
        [SerializeField]
        private Image m_profileImage;

        [SerializeField]
        private TextMeshProUGUI m_profileText;

        [SerializeField]
        private Button m_logOutButton;

        public Button LogOutButton => m_logOutButton;

        public Image ProfileImage => m_profileImage;

        public TextMeshProUGUI ProfileText => m_profileText;

        public void AddOnClickButtonsListener(UnityAction<Button> callback)
        {
            m_logOutButton.onClick.AddListener(() => callback(m_logOutButton));
        }

        public void SetProfileText(string text)
        {
            m_profileText.Set(text);
        }
    }
}
