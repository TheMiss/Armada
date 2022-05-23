using System.Collections.Generic;
using Armageddon.Mechanics.Mails;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.Events;

namespace Armageddon.UI.Common.MailModule
{
    public class MailboxCellView : EnhancedScrollerCellView
    {
        public delegate void SelectedDelegate(MailboxRowCellView rowCellView);

        [SerializeField]
        private List<MailboxRowCellView> m_rowCellViews;

        public List<MailboxRowCellView> RowCellViews => m_rowCellViews;

        public void SetMails(IReadOnlyList<Mail> mails, int startingIndex)
        {
            if (RowCellViews.Count == 0)
            {
                Debug.Log("m_rowCellViews.Count should be at least 1");
            }

            for (int i = 0; i < RowCellViews.Count; i++)
            {
                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                int slotIndex = startingIndex + i;
                RowCellViews[i].SetMail(slotIndex < mails.Count ? mails[slotIndex] : null);
            }
        }

        public void AddOnClickListener(UnityAction<MailboxRowCellView> callback)
        {
            foreach (MailboxRowCellView rowCellView in RowCellViews)
            {
                rowCellView.Button.onClick.AddListener(() => callback.Invoke(rowCellView));
            }
        }
    }
}
