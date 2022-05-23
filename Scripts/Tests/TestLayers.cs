using System.Diagnostics;
using Armageddon.Externals.OdinInspector;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Armageddon.Tests
{
    public class TestLayers : MonoBehaviour
    {
        public ContactFilter2D ContactFilter2D;

        public uint TestCount = 1000;

        [Button("Test")]
        [GUIColorDefaultButton]
        private void Test()
        {
            RunTest1();
            RunTest2();
        }

        private void RunTest1()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();


            for (int i = 0; i < TestCount; i++)
            {
                int test = LayerMaskEx.GetMask(Layers.Hero, Layers.Companion);
            }

            //for (int i = 0; i < TestCount; i++)
            //{
            //    int test = LayerMask.GetMask("Hero", "CompanionShop");
            //}

            stopWatch.Stop();

            Debug.Log($"RunTest1: {stopWatch.Elapsed} ({stopWatch.ElapsedMilliseconds})");
        }

        private void RunTest2()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();


            for (int i = 0; i < TestCount; i++)
            {
                int test = LayerMask.GetMask("Hero", "CompanionShop");
            }

            stopWatch.Stop();

            Debug.Log($"RunTest2: {stopWatch.Elapsed} ({stopWatch.ElapsedMilliseconds})");
        }
    }
}
