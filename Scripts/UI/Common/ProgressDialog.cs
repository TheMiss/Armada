using System;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common
{
    public class ProgressDialogReport
    {
        public ProgressDialogReport(TextMeshProUGUI messageText, Slider progressSlider)
        {
            MessageText = messageText;
            ProgressSlider = progressSlider;
        }

        public TextMeshProUGUI MessageText { get; }
        public Slider ProgressSlider { get; }

        public void SetText(string text)
        {
            MessageText.Set(text);
        }

        public void SetValue(float value)
        {
            ProgressSlider.value = value;
        }
    }

    public class ProgressDialog : AlertDialog
    {
        [SerializeField]
        private Slider m_progressSlider;

        public Slider ProgressSlider => m_progressSlider;

        private ProgressDialogReport ProgressDialogReport { set; get; }

        public async UniTask<bool?> ShowAsync(string titleText,
            Func<ProgressDialogReport, UniTask> loadingTask, string acceptButtonText = "Okay",
            string rejectButtonText = null)
        {
            TitleText.Set(titleText);
            AcceptButtonText.Set(acceptButtonText);
            AcceptButton.gameObject.SetActive(false);
            RejectButton.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(rejectButtonText))
            {
                RejectButton.gameObject.SetActive(true);
                RejectButtonText.Set(rejectButtonText);
            }

            transform.SetAsLastSibling();

            if (ProgressDialogReport == null)
            {
                ProgressDialogReport = new ProgressDialogReport(
                    MessageText, ProgressSlider);
            }

            Show();

            await CreateUniTask(loadingTask, ProgressDialogReport);

            AcceptButton.gameObject.SetActive(true);

            DialogResult = null;

            while (DialogResult == null)
            {
                await UniTask.Yield();
            }

            return DialogResult;
            // Causes double create dialog blocker.
            //return await ShowAsync(); // Call Show() again should not hurt.
        }

        private static UniTask CreateUniTask<T1>(Func<T1, UniTask> factory, T1 t1)
        {
            return factory(t1);
        }
    }
}
