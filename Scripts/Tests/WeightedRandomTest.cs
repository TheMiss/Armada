using System;
using System.Collections.Generic;
using System.Text;
using Purity.Common.Randomization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    [Serializable]
    public class WeightedAnimal : WeightedRandomEntry<Animal>
    {
        // TODO: Outdated, might not function properly now.
        public WeightedAnimal(Animal animal, float weight, Action weightChangedCallback)
        {
        }

        [PropertyOrder(-1)]
        [ShowInInspector]
        public string Name => Value != null ? Value.GetType().Name : string.Empty;

        // public override Animal Object { get; set; }
    }

    public class WeightedRandomTest : MonoBehaviour
    {
        [TableList]
        public List<WeightedAnimal> Animals;

        private void OnValidate()
        {
            WeightedRandom<Animal>.CalculateOdds(Animals);
        }

        [Button]
        private void RefreshAnimals()
        {
            Animals = new List<WeightedAnimal>
            {
                new WeightedAnimal(new Dog(), 1, WeightChangedCallback),
                new WeightedAnimal(new Cat(), 1, WeightChangedCallback),
                new WeightedAnimal(new Horse(), 1, WeightChangedCallback),
                new WeightedAnimal(new Bird(), 1, WeightChangedCallback),
                new WeightedAnimal(new Lion(), 1, WeightChangedCallback)
            };
        }

        private void WeightChangedCallback()
        {
            OnValidate();
        }

        [Button]
        private void RunAnimalTest1()
        {
            Dictionary<Animal, int> samplingAnimals = WeightedRandom<Animal>.Sample(Animals);

            var stringBuilder = new StringBuilder();
            foreach (KeyValuePair<Animal, int> samplingAnimal in samplingAnimals)
            {
                stringBuilder.Append($"{samplingAnimal.Key} count: {samplingAnimal.Value}\n");
            }

            Debug.Log(stringBuilder.ToString());
        }

        [Button]
        private void RunAnimalTest2()
        {
            var animalRandom = new WeightedRandom<Animal>(Animals);
            Dictionary<Animal, int> samplingAnimals = animalRandom.Sample();

            var stringBuilder = new StringBuilder();
            foreach (KeyValuePair<Animal, int> samplingAnimal in samplingAnimals)
            {
                stringBuilder.Append($"{samplingAnimal.Key} count: {samplingAnimal.Value}\n");
            }

            Debug.Log(stringBuilder.ToString());
        }
    }
}
