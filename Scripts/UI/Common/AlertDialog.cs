using System;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon.UI.Common
{
    public enum AlertDialogMode
    {
        Information,
        Warning,
        Error
    }

    public class AlertDialog : Dialog
    {
        [SerializeField]
        private bool m_logMessage;

        public bool LogOnShow
        {
            get => m_logMessage;
            set => m_logMessage = value;
        }

        [ShowInPlayMode]
        public AlertDialogMode Mode { get; private set; }

        public async UniTask<bool?> ShowInfoDialogAsync(string titleText, string messageText,
            string acceptButtonText, string rejectButtonText = null, Action<bool> resultCallback = null)
        {
            return await ShowDialogAsync(AlertDialogMode.Information, titleText, messageText,
                acceptButtonText, rejectButtonText, resultCallback);
        }

        public async UniTask<bool?> ShowWarningDialogAsync(string titleText, string messageText,
            string acceptButtonText, string rejectButtonText = null, Action<bool> resultCallback = null)
        {
            return await ShowDialogAsync(AlertDialogMode.Warning, titleText, messageText,
                acceptButtonText, rejectButtonText, resultCallback);
        }

        public async UniTask<bool?> ShowErrorDialogAsync(string titleText, string messageText,
            string acceptButtonText, string rejectButtonText = null, Action<bool> resultCallback = null)
        {
            return await ShowDialogAsync(AlertDialogMode.Error, titleText, messageText,
                acceptButtonText, rejectButtonText, resultCallback);
        }

        public async UniTask<bool?> ShowDialogAsync(AlertDialogMode mode, string titleText, string messageText,
            string acceptButtonText, string rejectButtonText = null, Action<bool> resultCallback = null)
        {
            Mode = mode;
            SetTitleText(titleText);
            SetMessageText(messageText);
            AcceptButtonText.Set(acceptButtonText);
            RejectButton.gameObject.SetActive(false);

            if (LogOnShow)
            {
                switch (Mode)
                {
                    case AlertDialogMode.Information:
                        Debug.Log(messageText);
                        break;
                    case AlertDialogMode.Warning:
                        Debug.LogWarning(messageText);
                        break;
                    case AlertDialogMode.Error:
                        Debug.LogError(messageText);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!string.IsNullOrEmpty(rejectButtonText))
            {
                RejectButton.gameObject.SetActive(true);
                RejectButtonText.Set(rejectButtonText);
            }

            bool? result = await ShowDialogAsync();

            if (result != null)
            {
                resultCallback?.Invoke(result.Value);
            }

            return result;
        }
    }
}
