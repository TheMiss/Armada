using Armageddon.Localization;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.Common
{
    public class WaitForRetryDialog : AlertDialog
    {
        [SerializeField]
        private TextMeshProUGUI m_timerText;

        public void SetDefaultTexts()
        {
            TitleText.Set($"{Texts.UI.Error}!");
            MessageText.Set(Texts.Message.YouSentTooManyRequests);
            AcceptButtonText.Set(Texts.UI.GotIt);
        }

        private async UniTaskVoid CountDown(float timer)
        {
            while (timer > 0)
            {
                m_timerText.Set($"{(int)timer} s");
                await UniTask.Delay(1000);
                timer -= 1.0f;
            }

            m_timerText.gameObject.SetActive(false);
        }

        public async UniTask<bool?> ShowDialogAsync(float delay)
        {
            CountDown(delay).Forget();

            RejectButton.gameObject.SetActive(false);

            return await ShowDialogAsync();
        }
    }
}
