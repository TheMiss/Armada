using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class Calculation : MonoBehaviour
    {
        public string ResultString;
        public List<float> sources;

        [Button]
        private void GetResult()
        {
            float sum = 1;
            foreach (float source in sources)
            {
                sum *= 1 - source / 100f;
            }

            float result = 1 - sum;
            ResultString = $"{result:F3} ({result:P})";
        }

        [Button]
        private void GetResult2()
        {
            float sum = 1;
            foreach (float source in sources)
            {
                sum *= 1 + source / 100f;
            }

            float result = sum;
            ResultString = $"{result:F3} ({result:P})";
        }

        [Button]
        private void GetResult3()
        {
            var orderedSources = new List<float>(sources);
            orderedSources.Sort((x, y) => x < y ? -1 : 1);
            orderedSources.Reverse();

            float sum = 0;
            for (int i = 0; i < orderedSources.Count; i++)
            {
                float source = orderedSources[i];
                float add = source / 100f;

                if (i > 0)
                {
                    add *= i * 0.25f;
                }

                sum += add;
            }

            float result = sum;
            ResultString = $"{result:F3} ({result:P}) [{orderedSources.Sum() / 100f:P}]";
        }
    }
}
