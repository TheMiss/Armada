using System;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    /// <summary>
    ///     A dialog window is a top-level window mostly used for short-term tasks and brief communications with the user
    /// </summary>
    public abstract class Dialog : Window
    {
        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_messageText;

        [SerializeField]
        private Button m_rejectButton;

        [SerializeField]
        private bool m_customizeRejectButtonText;

        [ShowIf(nameof(m_customizeRejectButtonText))]
        [SerializeField]
        private TextMeshProUGUI m_rejectButtonText;

        [SerializeField]
        private Button m_acceptButton;

        [SerializeField]
        private bool m_customizeAcceptButtonText;

        [ShowIf(nameof(m_customizeAcceptButtonText))]
        [SerializeField]
        private TextMeshProUGUI m_acceptButtonText;

        public TextMeshProUGUI TitleText => m_titleText;

        public TextMeshProUGUI MessageText => m_messageText;

        public TextMeshProUGUI RejectButtonText => m_rejectButtonText;

        public TextMeshProUGUI AcceptButtonText => m_acceptButtonText;

        public Button AcceptButton => m_acceptButton;

        public Button RejectButton => m_rejectButton;


        protected override void Awake()
        {
            base.Awake();

            if (m_rejectButton != null)
            {
                m_rejectButton.onClick.AddListener(OnRejectButtonClicked);
            }

            if (m_acceptButton != null)
            {
                m_acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            }
        }

        protected virtual void OnRejectButtonClicked()
        {
            DialogResult = false;
        }

        protected virtual void OnAcceptButtonClicked()
        {
            DialogResult = true;
        }

        public void SetTitleText(string text)
        {
            TitleText.Set(text);
        }

        public void SetMessageText(string text)
        {
            MessageText.Set(text);
        }

        public void SetRejectButtonText(string text)
        {
            // No null checking here
            RejectButtonText.Set(text);
        }

        public void SetAcceptButtonText(string text)
        {
            // No null checking here
            AcceptButtonText.Set(text);
        }

        public void SetButtonTexts(string acceptButtonText, string rejectButtonText)
        {
            SetAcceptButtonText(acceptButtonText);
            SetRejectButtonText(rejectButtonText);
        }

        public async UniTask<bool?> ShowDialogAsync(Action<bool> resultCallback)
        {
            bool? result = await ShowDialogAsync();

            if (result != null)
            {
                resultCallback?.Invoke(result.Value);
            }

            return result;
        }
    }
}
