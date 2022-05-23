using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Purity.Common;
using UnityEngine;

namespace Armageddon.UI.Base
{
    public class Window : Widget
    {
        public Color BlockerColor = new(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        [ShowInPlayMode]
        private Transform m_previousParent;

        private bool m_hasBlocker;

        [ShowInPlayMode]
        public int BlockerSiblingIndex { get; set; }

        protected bool? DialogResult { get; set; }

        public Action<Window> TransformIndexChanged { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
#if UNITY_EDITOR
            _SetTextMeshObjectNames();
#endif
        }
        
        /// <summary>
        ///     Show as modal or modeless window.
        /// </summary>
        public async UniTask<bool?> ShowDialogAsync(bool animate = true, bool modal = true,
            bool closeWhenClickOnBlocker = false, Transform blockerParent = null)
        {
            m_previousParent = Transform.parent;

            if (Transform.parent != UI.CommonTransform)
            {
                Transform.SetParent(UI.CommonTransform, true);
            }

            Transform.SetAsLastSibling();

            if (modal)
            {
                Blocker blocker = AddBlocker(blockerParent, BlockerColor, () =>
                {
                    if (closeWhenClickOnBlocker)
                    {
                        DialogResult = false;
                    }
                });

                if (blockerParent != null)
                {
                    // blocker.Transform.SetParent(blockerParent);
                    blocker.Transform.SetAsFirstSibling();
                    // blocker.Transform.SetSiblingIndex(BlockerSiblingIndex);
                }

                m_hasBlocker = true;
            }

            TransformIndexChanged?.Invoke(this);
            TransformIndexChanged = null;

            Show(animate);

            DialogResult = null;
            CancellationToken token = GetCancellationToken(nameof(ShowDialogAsync));

            while (DialogResult == null)
            {
                await UniTask.Yield(token);
            }

            Hide(animate);
            EndDialog();

            return DialogResult;
        }

        private void EndDialog()
        {
            if (m_hasBlocker)
            {
                RemoveBlocker();
                m_hasBlocker = false;
            }

            if (Transform.parent != m_previousParent)
            {
                m_previousParent = Transform.parent;
                Transform.SetParent(m_previousParent, true);
            }
        }

        public void CloseDialog()
        {
            DialogResult = true;

            EndDialog();
        }

        public async UniTask<bool?> ShowDialogAsync(Transform blockerParent)
        {
            return await ShowDialogAsync(true, true, false, blockerParent);
        }
    }
}
