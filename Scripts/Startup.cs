using System;
using System.Collections;
using System.Collections.Generic;
using Armageddon.Sheets.Items;
using Cysharp.Threading.Tasks;
using Purity.Common;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Armageddon
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_messageText;

        [SerializeField]
        private Slider m_loadingProgressBar;

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            var stopwatch = new Stopwatch("Startup.Start");

            m_messageText.text = "Starting up...";
            m_loadingProgressBar.value = 0.0f;
            AsyncOperation operation = SceneManager.LoadSceneAsync("Main");
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                m_loadingProgressBar.value = operation.progress / 0.9f;

                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            stopwatch.Stop();
        }

        // private void Start()
        // {
        //     StartAsync().Forget();
        //
        //     // StartCoroutine(Assets.PreloadHazards());
        //
        //     // Start2Async().Forget();
        // }

        private async UniTask Start2Async()
        {
            var stopwatch = new Stopwatch("Startup.Start");

            Action<float> handler = Handler;
            IProgress<float> progress = Progress.Create(handler);

            IList<ItemSheet> itemSheets = await Addressables.LoadAssetsAsync<ItemSheet>("Item", Callback)
                .ToUniTask(progress);

            // var itemSheets = await Addressables.LoadAssetsAsync<ItemSheet>("Item", Callback)
            //     .ToUniTask(Progress.Create<float>(x => Debug.Log(x)));
            //
            await SceneManager.LoadSceneAsync("Main");

            stopwatch.Stop();
        }

        private void Handler(float x)
        {
            Debug.Log(x);
        }

        private void Callback(ItemSheet obj)
        {
            Debug.Log(obj.Name);
        }

        // private async UniTask StartAsync()
        // {
        //     var stopwatch = new Stopwatch("Startup.Start");
        //     
        //     m_messageText.text = "Starting up...";
        //     m_loadingProgressBar.value = 0.0f;
        //     
        //     // await as it won't finish before using it in Startup scene.
        //     // even if we use [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //     await Assets.InitializeAsync();
        //
        //     await Assets.LoadAlwaysInMemoryResources(tuple =>
        //     {
        //         (float progress, string assetName) = tuple;
        //         m_loadingProgressBar.value = progress;
        //         m_messageText.text = $"Loaded {assetName}";
        //         // Debug.Log(progress);
        //     });
        //
        //     await SceneManager.LoadSceneAsync("Main");
        //
        //     stopwatch.Stop();
        // }

        // private async UniTask StartAsync()
        // {
        //     var stopwatch = new Stopwatch("Startup.Start");
        //     
        //     m_messageText.text = "Starting up...";
        //     m_loadingProgressBar.value = 0.0f;
        //     
        //     // await as it won't finish before using it in Startup scene.
        //     // even if we use [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //     await Assets.InitializeAsync();
        //
        //     Assets.LoadAlwaysInMemoryResources(tuple =>
        //     {
        //         (float progress, string assetName) = tuple;
        //         m_loadingProgressBar.value = progress;
        //         Debug.Log(progress);
        //         // await UniTask.WaitForEndOfFrame();
        //     }).Forget();
        //
        //     // if (m_loadingProgressBar.value >= 0.99f)
        //     // {
        //     // }
        //
        //     while (m_loadingProgressBar.value <= 0.99f)
        //     {
        //         await UniTask.Yield();
        //     }
        //
        //     // await SceneManager.LoadSceneAsync("Main");
        //
        //     stopwatch.Stop();
        // }
    }
}
