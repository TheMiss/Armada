using System.Collections.Generic;
using System.Threading;
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
    public class MailboxRowCellView : Widget
    {
        private const int MaxPreviewAttachedItemCount = 8;
        
        [BoxGroupPrefabs]
        [SerializeField]
        private MailAttachedItemWidget m_mailAttachedItemWidgetPrefab;

        [SerializeField]
        private Button m_button;

        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_messageText;

        [SerializeField]
        private TextMeshProUGUI m_expiresAfterText;

        [SerializeField]
        private TextMeshProUGUI m_elapsedTimeText;

        [SerializeField]
        private RectTransform m_attachedItemsContentTransform;

        [SerializeField]
        private List<MailAttachedItemWidget> m_attachedItems;

        public Button Button => m_button;

        [ShowInPlayMode]
        public Mail Mail { get; private set; }
        
        public override string ToString()
        {
            return m_titleText.text;
        }

        public void SetMail(Mail mail)
        {
            if (mail == null)
            {
                // For email, there should not be null.
                return;
            }

            Mail = mail;
            m_titleText.Set(mail.Title);
            SetExpiresAfterText();
            SetElapsedTimeText();
            SetBadge(Mail.IsNoticed);
            
            if (mail.AttachedItems.Count > 0)
            {
                m_messageText.gameObject.SetActive(false);
                m_attachedItemsContentTransform.gameObject.SetActive(true);

                SetAttachedItems(mail.AttachedItems);
            }
            else
            {
                m_attachedItemsContentTransform.gameObject.SetActive(false);
                m_messageText.gameObject.SetActive(true);
                m_messageText.Set(mail.Message);

                m_attachedItemsContentTransform.gameObject.SetActive(false);
            }

            StopAllUniTasks();
            UpdateDateTimeAsync().Forget();
        }

        private void SetExpiresAfterText()
        {
            m_expiresAfterText.Set(Lexicon.ExpiresAfter(Mail.ExpiredDateTime));
        }

        private void SetElapsedTimeText()
        {
            m_elapsedTimeText.Set(Lexicon.TimePassedAgo(Mail.ReceivedDateTime));
        }

        private async UniTask UpdateDateTimeAsync()
        {
            CancellationToken token = GetCancellationToken(nameof(UpdateDateTimeAsync));

            while (gameObject.activeInHierarchy)
            {
                SetExpiresAfterText();
                SetElapsedTimeText();

                // TODO: Remove when it expires.

                await UniTask.Delay(1000, true, cancellationToken: token);
            }
        }

        private void SetAttachedItems(List<MailAttachedItem> attachedItems)
        {
            // m_attachedItems was non serializable before...
            if (m_attachedItems == null)
            {
                m_attachedItems = new List<MailAttachedItemWidget>();

                // 8 is the maximum it can display.
                for (int i = 0; i < MaxPreviewAttachedItemCount; i++)
                {
                    MailAttachedItemWidget mailAttachedItemWidget = Instantiate(m_mailAttachedItemWidgetPrefab,
                        m_attachedItemsContentTransform, true);

                    m_attachedItems.Add(mailAttachedItemWidget);
                }
            }

            foreach (MailAttachedItemWidget attachedItem in m_attachedItems)
            {
                attachedItem.gameObject.SetActive(false);
            }

            for (int i = 0; i < attachedItems.Count; i++)
            {
                if (i >= MaxPreviewAttachedItemCount)
                {
                    break;
                }

                MailAttachedItem attachedItem = attachedItems[i];
                MailAttachedItemWidget attachItemWidget = m_attachedItems[i];
                attachItemWidget.gameObject.SetActive(true);
                attachItemWidget.SetAttachItem(attachedItem);
            }

            // for (int i = 0; i < items.Count; i++)
            // {
            //     if (i >= MaxPreviewAttachedItemCount)
            //     {
            //         break;
            //     }
            //
            //     Item item = items[i];
            //     AttachedItem attachItem = m_attachedItems[i];
            //     attachItem.gameObject.SetActive(true);
            //     attachItem.ObjectHolder.SetItem(item);
            // }
        }
    }
}
