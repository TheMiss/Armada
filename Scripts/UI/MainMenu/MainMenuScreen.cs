using System.Diagnostics;
using Armageddon.UI.Base;
using Armageddon.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.UI.MainMenu
{
    public enum MainMenuBackgroundMode
    {
        Normal,
        WorldMap
    }

    public class MainMenuScreen : Widget
    {
        [SerializeField]
        private GameObject m_normalBackgroundObject;

        [SerializeField]
        private GameObject m_worldPageBackgroundObject;

        [SerializeField]
        private TabBar m_tabBar;

        public TabBar TabBar => m_tabBar;

        private void OnLoadoutButtonClicked()
        {
            var ui = GetService<UISystem>();
            ui.DisplayLoadout();
        }

        private void OnPlayButtonClicked()
        {
            TabBar.SetSelectedTab(1);
        }

        public void SetBackgroundEnabled(bool value)
        {
            m_normalBackgroundObject.SetActive(value);
        }

        private async UniTask LoadingTaskAsync(ProgressDialogReport report)
        {
            float loadingTime = 0;

            while (true)
            {
                loadingTime += Time.deltaTime;
                float percent = loadingTime / 3.0f;

                if (percent >= 1.0f)
                {
                    percent = 1.0f;
                }

                report.SetText($"loadingTime = {loadingTime:0.0} ({percent * 100.0f:0.00})%");
                report.SetValue(percent);

                if (percent >= 1.0f)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        private async UniTask LoadingTask2Async(ProgressDialogReport report)
        {
            var loadingTime = Stopwatch.StartNew();
            var stopwatch = Stopwatch.StartNew();
            int yieldCounter = 1;

            int maxCount = 1000000;
            for (int i = 0; i < maxCount; i++)
            {
                float percent = (float)i / maxCount;
                report.SetText(
                    $"i = {i} (avg = {i / yieldCounter}), loadingTime = {loadingTime.ElapsedMilliseconds / 1000f:0.0} ({percent * 100.0f:0.00})%");
                report.SetValue(percent);

                float elapsedTime = stopwatch.ElapsedMilliseconds / 1000.0f;

                if (elapsedTime > 1 / 15.0f)
                {
                    yieldCounter++;
                    stopwatch.Restart();
                    await UniTask.Yield();
                }
            }

            // while (true)
            // {
            //     loadingTime += Time.deltaTime;
            //     float percent = loadingTime / 3.0f;
            //
            //     if (percent >= 1.0f)
            //     {
            //         percent = 1.0f;
            //     }
            //
            //     report.SetText($"loadingTime = {loadingTime:0.0} ({percent * 100.0f:0.00})%");
            //     report.SetValue(percent);
            //
            //     if (percent >= 1.0f)
            //     {
            //         break;
            //     }
            //
            //     await UniTask.Yield();
            // }
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                // MainMenuBar.ToggleState();
            }
        }
    }
}
