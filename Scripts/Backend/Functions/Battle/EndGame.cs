using System.Collections.Generic;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class EndGameRequest : BackendRequest
    {
        public int StageId;
        public List<int> KilledEnemies;
        public List<int> ReceivedDrops;
    }

    [FunctionReply]
    public class EndGameReply : BackendReply
    {
        public int Level;
        public long Exp;
        public DropPayload[] Drops = { };
    }
}
