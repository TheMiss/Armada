using System.Collections.Generic;
using Armageddon.Backend.Payloads;

namespace Armageddon.Mechanics.Maps
{
    public class Map
    {
        public Map(MapPayload obj)
        {
            Id = obj.Id;

            foreach (StagePayload stageObject in obj.MainStages)
            {
                var stageOverview = new Stage(stageObject);
                Stages.Add(stageOverview);
            }
        }

        public int Id { get; }
        public List<Stage> Stages { get; } = new();
    }
}
