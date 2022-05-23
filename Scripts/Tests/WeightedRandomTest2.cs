using System.Collections.Generic;
using Purity.Common.Randomization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class WeightedRandomTest2 : MonoBehaviour
    {
        [TableList]
        public List<NameableWeightedRandomEntry<GameObject>> Objects;

        private void OnValidate()
        {
            WeightedRandom<GameObject>.CalculateOdds(Objects);
            WeightedRandomUtility.CalculateOdds(Objects);
        }
    }
}
