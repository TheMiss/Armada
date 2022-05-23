using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Armageddon.Tests
{
    public class CollectionTest : MonoBehaviour
    {
        public int CreateGameObjectCount = 10000;

        public List<Button> TestButtons;
        public TextMeshProUGUI TestResultTest;
        private readonly List<GameObject> m_readOnlyGameObjects = new List<GameObject>();
        public IReadOnlyList<GameObject> ReadOnlyGameObjects => m_readOnlyGameObjects;
        public List<GameObject> GameObjects { get; } = new List<GameObject>();

        private void Awake()
        {
            for (int i = 0; i < CreateGameObjectCount; i++)
            {
                var newObject = new GameObject($"i{i}");
                newObject.transform.SetParent(transform, true);

                m_readOnlyGameObjects.Add(newObject);
                GameObjects.Add(newObject);
            }

            for (int i = 0; i < TestButtons.Count; i++)
            {
                int index = i;
                TestButtons[i].onClick.AddListener(() => OnTestButtonClicked(index));
            }
        }

        private void Start()
        {
            Test();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Test();
            }
        }

        private void OnTestButtonClicked(int i)
        {
            if (i == 0)
            {
                Test();
            }
        }

        private void Test()
        {
            var stopwatch = Stopwatch.StartNew();
            foreach (GameObject readOnlyGameObject in ReadOnlyGameObjects)
            {
                Vector3 position = readOnlyGameObject.transform.position;
                position += Vector3.down;
                readOnlyGameObject.transform.position = position;
            }

            stopwatch.Stop();

            var stringBuilder = new StringBuilder();

            string log = $"foreach ReadOnlyGameObjects: {stopwatch.Elapsed}";
            stringBuilder.Append($"{log}\n");
            Debug.Log(log);

            // ===================================================================
            stopwatch.Restart();

            foreach (GameObject readOnlyGameObject in GameObjects)
            {
                Vector3 position = readOnlyGameObject.transform.position;
                position += Vector3.down;
                readOnlyGameObject.transform.position = position;
            }

            stopwatch.Stop();

            log = $"foreach GameObjects: {stopwatch.Elapsed}";
            stringBuilder.Append($"{log}\n");

            Debug.Log(log);

            // ===================================================================
            stopwatch.Restart();

            int readOnlyGameObjectsCount = ReadOnlyGameObjects.Count;
            for (int i = 0; i < readOnlyGameObjectsCount; i++)
            {
                GameObject readOnlyGameObject = ReadOnlyGameObjects[i];
                Vector3 position = readOnlyGameObject.transform.position;
                position += Vector3.down;
                readOnlyGameObject.transform.position = position;
            }

            stopwatch.Stop();

            log = $"for ReadOnlyGameObjects: {stopwatch.Elapsed}";
            stringBuilder.Append($"{log}\n");

            Debug.Log(log);

            // ===================================================================
            stopwatch.Restart();

            int gameObjectsCount = GameObjects.Count;
            for (int i = 0; i < gameObjectsCount; i++)
            {
                GameObject gameObject = GameObjects[i];
                Vector3 position = gameObject.transform.position;
                position += Vector3.down;
                gameObject.transform.position = position;
            }

            stopwatch.Stop();

            log = $"for GameObjects: {stopwatch.Elapsed}";
            stringBuilder.Append($"{log}\n");

            Debug.Log(log);

            TestResultTest.text = stringBuilder.ToString();
        }
    }
}
