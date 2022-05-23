using System.Collections.Generic;
using System.IO;
using System.Text;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class RandomTest : MonoBehaviour
    {
        public int Seed;
        public int GiveCount = 10;
        public List<double> DoubleResults;
        public List<long> IntResults;
        public int IntMin = 50;
        public int IntMax = 70;

        [Button]
        private void GiveMe()
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < GiveCount; i++)
            {
                double result = CustomRandom.Range(50, 70.0f);
                DoubleResults.Add(result);

                //stringBuilder.Append($"Result = {result}\n");
            }

            stringBuilder.Append($"CallCountSinceLastSetSeed = {CustomRandom.CallCountSinceLastSetSeed}\n");

            CustomRandom.Seed = 1024;

            for (int i = 0; i < GiveCount; i++)
            {
                long result = CustomRandom.Range(IntMin, IntMax);
                IntResults.Add(result);

                stringBuilder.Append($"Result = {result}\n");
            }

            var counters = new Dictionary<int, int>();

            for (int i = IntMin; i < IntMax; i++)
            {
                counters.Add(i, 0);
            }

            stringBuilder.Append($"CallCountSinceLastSetSeed = {CustomRandom.CallCountSinceLastSetSeed}\n");

            foreach (int intResult in IntResults)
            {
                counters[intResult] = counters[intResult] + 1;
            }

            stringBuilder.Append("** Let's see the average **\n");


            foreach (int key in counters.Keys)
            {
                float percent = (float)counters[key] / IntResults.Count * 100.0f;
                stringBuilder.Append($"{key} = {counters[key]} ({percent}%)\n");
            }

            Debug.Log(stringBuilder.ToString());
        }

        [Button]
        private void ResetMe()
        {
            CustomRandom.Seed = Seed;
            DoubleResults = new List<double>();
            IntResults = new List<long>();

            //SyncRandom.Seed = DateTime.Now.Millisecond;
        }

        [Button]
        private void GiveMeTest()
        {
            var stringBuilder = new StringBuilder();

            int oldSeed = CustomRandom.Seed;

            CustomRandom.Seed = 512;
            stringBuilder.Append($"ServerRandom.Seed = {CustomRandom.Seed}\n");

            for (int i = 0; i < 10; i++)
            {
                double x = CustomRandom.Range(50, 70.0f);
                stringBuilder.Append($"x = {x}\n");
            }

            stringBuilder.Append($"CallCountSinceLastSetSeed = {CustomRandom.CallCountSinceLastSetSeed}\n");

            CustomRandom.Seed = 1024;
            stringBuilder.Append($"ServerRandom.Seed = {CustomRandom.Seed}\n");

            for (int i = 0; i < 15; i++)
            {
                long x = CustomRandom.Range(50, 70);
                stringBuilder.Append($"x = {x}\n");
            }

            stringBuilder.Append($"CallCountSinceLastSetSeed = {CustomRandom.CallCountSinceLastSetSeed}\n");

            CustomRandom.Seed = oldSeed;

            Debug.Log(stringBuilder.ToString());
        }

        [Button]
        private void RunTest1()
        {
            int[] globalResults = new int[100];

            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Results.txt"), string.Empty);

            for (int i = 0; i < globalResults.Length; i++)
            {
                globalResults[i] = 0;
            }

            for (int i = 0; i < 9999999; i++)
            {
                CustomRandom.Seed = i;

                int[] results = new int[100];

                for (int j = 0; j < results.Length; j++)
                {
                    results[j] = 0;
                }

                const int runTestCount = 1000;

                for (int runTestIndex = 0; runTestIndex < runTestCount; runTestIndex++)
                {
                    long index = CustomRandom.Range(0, results.Length);
                    results[index]++;
                    globalResults[index]++;
                }

                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"Run Test with seed <{i}>\n");

                    int totalSum = 0;

                    for (int j = 0; j < results.Length; j++)
                    {
                        int resultValue = results[j];
                        totalSum += resultValue;

                        stringBuilder.Append($"results[{j}] = {resultValue}\n");
                    }

                    int average = totalSum / results.Length;

                    stringBuilder.Append($"average = {average}\n");
                    stringBuilder.Append("............\n\n");

                    File.AppendAllText(Path.Combine(Application.persistentDataPath, "Results.txt"),
                        stringBuilder.ToString());
                    // Debug.Log(stringBuilder.ToString());
                }
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append($"Run Test Global Result at seed <{i}>\n");

                    for (int j = 0; j < results.Length; j++)
                    {
                        int globalResult = globalResults[j];

                        stringBuilder.Append($"results[{j}] = {globalResult}\n");
                    }

                    stringBuilder.Append("............\n");

                    File.WriteAllText(Path.Combine(Application.persistentDataPath, "GlobalResults.txt"),
                        stringBuilder.ToString());
                    // Debug.LogWarning(stringBuilder.ToString());
                }
            }
        }
    }
}
