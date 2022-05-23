using System.Collections.Generic;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class CompareStringTest : MonoBehaviour
    {
        public int TestCount = 10000;

        [Button]
        private void Execute()
        {
            var stopwatch = new Stopwatch("Create Data: Number");

            var numbers = new List<int>();

            for (int i = 0; i < TestCount; i++)
            {
                numbers.Add(i);
            }

            stopwatch.Stop();

            stopwatch = new Stopwatch("Test Data: Number");
            foreach (int number in numbers)
            {
                if (number == 500)
                {
                }
            }

            stopwatch.Stop();

            stopwatch = new Stopwatch("Create Data: String");
            var numberStrings = new List<string>();

            for (int i = 0; i < TestCount; i++)
            {
                numberStrings.Add($"{i}");
            }

            stopwatch.Stop();

            stopwatch = new Stopwatch("Test Data: String");
            foreach (string number in numberStrings)
            {
                if (number == "500")
                {
                }
            }

            stopwatch.Stop();

            stopwatch = new Stopwatch("Create Data: Long String");
            var numberLongStrings = new List<string>();

            for (int i = 0; i < TestCount; i++)
            {
                numberLongStrings.Add($"AABBCCDDEEFFGG{i}");
            }

            stopwatch.Stop();

            stopwatch = new Stopwatch("Test Data: String");
            foreach (string number in numberLongStrings)
            {
                if (number == "AABBCCDDEEFFGG500")
                {
                }
            }

            stopwatch.Stop();
        }
    }
}
