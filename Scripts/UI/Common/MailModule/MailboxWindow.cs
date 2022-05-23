using System;
using System.Collections.Generic;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Mails;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Purity.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.MailModule
{
    public class MailboxWindow : Window, IEnhancedScrollerDelegate
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private EnhancedScrollerCellView m_cellViewPrefab;

        [SerializeField]
        private EnhancedScroller m_scroller;

        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private Button m_deleteAllButton;

        [SerializeField]
        private Button m_claimAllButton;

        [SerializeField]
        private int m_numberOfCellsPerRow = 1;

        [SerializeField]
        private int m_cellSize = 270;

        [ShowInPlayMode]
        private List<Mail> m_mails;

        protected override void Awake()
        {
            base.Awake();

            m_backButton.onClick.AddListener(OnBackButtonClicked);
            m_deleteAllButton.onClick.AddListener(OnDeleteAllButtonClicked);
            m_claimAllButton.onClick.AddListener(OnClaimAllButtonClicked);

            m_scroller.Delegate = this;
            m_scroller.cellViewInstantiated = (scroller, view) =>
            {
                var cellView = (MailboxCellView)view;
                cellView.AddOnClickListener(OnCellViewClicked);
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            RefreshMails();
        }

        int IEnhancedScrollerDelegate.GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt(m_mails.Count / (float)m_numberOfCellsPerRow);
        }

        float IEnhancedScrollerDelegate.GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_cellSize;
        }

        EnhancedScrollerCellView IEnhancedScrollerDelegate.GetCellView(EnhancedScroller scroller, 
            int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            var cellView = (MailboxCellView)scroller.GetCellView(m_cellViewPrefab);

            // data index of the first sub cell
            int rowIndex = dataIndex * m_numberOfCellsPerRow;
            cellView.name = $"Cell {rowIndex} to {rowIndex + m_numberOfCellsPerRow - 1}";

            // pass in a reference to our data set with the offset for this cell
            cellView.SetMails(m_mails, rowIndex);

            // return the cell to the scroller
            return cellView;
        }

        private void RefreshMails()
        {
            m_mails = Game.Player.Mails;
            m_scroller.ReloadData();
        }

        private void OnBackButtonClicked()
        {
            Hide();
            CloseDialog();
        }

        private void OnDeleteAllButtonClicked()
        {
            async UniTask Async()
            {
                Debug.Log($"Delete all mails");
                
                await Game.Player.DeleteAllMailAsync();
                    
                m_scroller.ReloadData();
            }
            
            Async().Forget();
        }

        private void OnClaimAllButtonClicked()
        {
            async UniTask Async()
            {
                Debug.Log($"Claiming all claimable mails");
                
                int index = m_scroller.GetCellViewIndexAtPosition(m_scroller.ScrollPosition);

                await Game.Player.ClaimAllMailsAsync();
                    
                m_scroller.ReloadData();
                m_scroller.JumpToDataIndex(index);
            }
            
            Async().Forget();
        }

        private void OnCellViewClicked(MailboxRowCellView cellView)
        {
            Debug.Log($"Clicked on {cellView}");

            OpenMailAsync(cellView).Forget();
        }

        private async UniTask OpenMailAsync(MailboxRowCellView cellView)
        {
            Mail mail = cellView.Mail;
            OpenMailDialog openMailDialog = UI.OpenMailDialog;
            OpenMailDialogResult? result = await openMailDialog.OpenAsync(mail);

            switch (result)
            {
                case OpenMailDialogResult.Claim:
                {
                    Debug.Log($"Claiming {mail}");
                    
                    int index = m_scroller.GetCellViewIndexAtPosition(m_scroller.ScrollPosition);
                    
                    await Game.Player.ClaimMailAsync(mail);
                    
                    m_scroller.ReloadData();
                    m_scroller.JumpToDataIndex(index);
                }
                    break;
                case OpenMailDialogResult.Delete:
                {
                    Debug.Log($"Deleting {mail}");
           
                    int index = m_scroller.GetCellViewIndexAtPosition(m_scroller.ScrollPosition);
                    
                    await Game.Player.DeleteMailAsync(mail);
                    
                    m_scroller.ReloadData();
                    m_scroller.JumpToDataIndex(index);
                }
                    break;
                case OpenMailDialogResult.Close:
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Refresh the badge.
            mail.IsNoticed = true;
            cellView.SetMail(mail);
        }
    }
}
