using System;
using System.Collections.Generic;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class ScoreRequirementPayload
    {
        public RewardRequirementType Type;
        public float Percent;
    }

    [Exchange]
    [Serializable]
    public class StagePayload
    {
        public int Id;
        public int StarCount;
        public int EnergyCost;
        public List<ScoreRequirementPayload> ScoreRequirements;
        public List<RewardPayload> Rewards;
        public List<int> PossibleEncounters;
    }
}
