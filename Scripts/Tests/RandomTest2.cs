using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class RandomTest2 : MonoBehaviour
    {
        public int Seed = 512;
        public int GiveCount = 100;
        public int IntMin = 50;
        public int IntMax = 100;
        public int IntAverage = 80;
        public bool ShowRandomValues;

        [Button]
        private void GiveMeTest()
        {
            var stringBuilder = new StringBuilder();

            int oldSeed = CustomRandom.Seed;

            CustomRandom.Seed = Seed;
            stringBuilder.Append($"ServerRandom.Seed = {CustomRandom.Seed}\n");
            stringBuilder.Append(
                $"IntMin = {IntMin}, " +
                $"IntMax = {IntMax}, " +
                $"IntAverage = {IntAverage}, \n");

            long total = 0;
            var numberDictionary = new Dictionary<long, int>();
            for (int i = 0; i < GiveCount; i++)
            {
                long x = CustomRandom.RangeBellCurve(IntMin, IntMax);
                total += x;

                if (ShowRandomValues)
                {
                    stringBuilder.Append($"x = {x}\n");
                }

                if (!numberDictionary.TryGetValue(x, out int count))
                {
                    numberDictionary.Add(x, 0);
                }

                numberDictionary[x] = ++count;
            }

            stringBuilder.Append($"CallCountSinceLastSetSeed = {CustomRandom.CallCountSinceLastSetSeed}\n");
            stringBuilder.Append($"total = {total}\n");
            stringBuilder.Append($"average = {total / GiveCount}\n");

            var numbers = new List<Tuple<long, int>>();

            foreach (KeyValuePair<long, int> kvp in numberDictionary)
            {
                numbers.Add(new Tuple<long, int>(kvp.Key, kvp.Value));
            }

            numbers.Sort();
            //numbers.Sort((x,y) => x.Item2 > y.Item2 ? -1 : 1);

            foreach ((long item1, int item2) in numbers)
            {
                stringBuilder.Append($"{item1} = {item2}\n");
            }

            CustomRandom.Seed = oldSeed;

            string path = Path.Combine(Application.persistentDataPath, "RandomTest2.txt");
            File.WriteAllText(path, stringBuilder.ToString());
            //EditorUtility.RevealInFinder(path);

            // Debug.Log(stringBuilder.ToString());
        }
    }
}
