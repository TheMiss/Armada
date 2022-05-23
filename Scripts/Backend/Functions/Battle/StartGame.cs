using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class StartGameRequest : BackendRequest
    {
        public int MapId;
        public int StageId;
    }

    [FunctionReply]
    public class StartGameReply : BackendReply
    {
        public int Energy;
        public HeroPayload Hero;
        public HeroInventoryPayload HeroInventory;
        public BattleStagePayload BattleStage;
    }
}
