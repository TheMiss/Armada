using System;
using System.Collections.Generic;
using System.Threading;
using Armageddon.Assistance.Designs;
using Armageddon.Externals.OdinInspector;
using Armageddon.Localization;
using Armageddon.Mechanics.Mails;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.MailModule
{
    public enum OpenMailDialogResult
    {
        Claim,
        Delete,
        Close
    }

    public class OpenMailDialog : Dialog
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private MailAttachedItemWidget m_mailAttachedItemWidgetPrefab;

        [SerializeField]
        private TextMeshProUGUI m_expiresAfterText;

        [SerializeField]
        private Button m_deleteButton;

        [SerializeField]
        private GameObject m_attachmentPanelObject;

        [SerializeField]
        private RectTransform m_attachedItemsContentTransform;

        [ShowInPlayMode]
        private List<MailAttachedItemWidget> m_attachedItems;

        private OpenMailDialogResult? m_result;

        [ShowInPlayMode]
        public Mail Mail { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            m_deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        protected override void OnRejectButtonClicked()
        {
            base.OnRejectButtonClicked();

            m_result = OpenMailDialogResult.Close;
        }

        protected override void OnAcceptButtonClicked()
        {
            base.OnAcceptButtonClicked();
            
            m_result = OpenMailDialogResult.Claim;
        }

        private void OnDeleteButtonClicked()
        {
            DialogResult = true;
            m_result = OpenMailDialogResult.Delete;
        }

        private void SetAttachedItems(List<MailAttachedItem> attachedItems)
        {
            if (m_attachedItems == null)
            {
                m_attachedItemsContentTransform.gameObject.DestroyDesignRemnant();
                m_attachedItems = new List<MailAttachedItemWidget>();
            }

            while (m_attachedItems.Count < attachedItems.Count)
            {
                MailAttachedItemWidget mailAttachedItemWidget =
                    Instantiate(m_mailAttachedItemWidgetPrefab, m_attachedItemsContentTransform);
                m_attachedItems.Add(mailAttachedItemWidget);
            }

            foreach (MailAttachedItemWidget objectHolderItem in m_attachedItems)
            {
                objectHolderItem.gameObject.SetActive(false);
            }

            for (int i = 0; i < attachedItems.Count; i++)
            {
                MailAttachedItemWidget mailAttachedItemWidget = m_attachedItems[i];
                mailAttachedItemWidget.gameObject.SetActive(true);
                mailAttachedItemWidget.SetAttachItem(attachedItems[i]);
                // objectHolderItem.ObjectHolder.SetItem(attachedItems[i]);
            }

            if (attachedItems.Count > 0)
            {
                AcceptButton.gameObject.SetActive(true);
                m_attachmentPanelObject.SetActive(true);
            }
            else
            {
                AcceptButton.gameObject.SetActive(false);
                m_attachmentPanelObject.SetActive(false);
            }
        }

        private void SetExpiresAfterText(DateTime expiredDateTime)
        {
            m_expiresAfterText.Set(Lexicon.ExpiresAfter(expiredDateTime));
        }

        private async UniTask UpdateDateTimeAsync()
        {
            CancellationToken token = GetCancellationToken(nameof(UpdateDateTimeAsync));

            while (m_result == null)
            {
                SetExpiresAfterText(Mail.ExpiredDateTime);

                // TODO: Remove when it expires.

                await UniTask.Delay(1000, true, cancellationToken: token);
            }
        }

        public async UniTask<OpenMailDialogResult?> OpenAsync(Mail mail)
        {
            Mail = mail;

            SetTitleText(mail.Title);
            SetMessageText(mail.Message);
            SetAttachedItems(mail.AttachedItems);
            SetExpiresAfterText(mail.ExpiredDateTime);

            StopAllUniTasks();
            UpdateDateTimeAsync().Forget();

            m_result = null;

            await ShowDialogAsync();

            return m_result;
        }
    }
}
