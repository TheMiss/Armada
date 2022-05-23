using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class SelectHeroRequest : BackendRequest
    {
        public int HeroSheetId;
        public string HeroInstanceId;
    }

    [FunctionReply]
    public class SelectHeroReply : BackendReply
    {
        public int HeroSheetId;
        public string HeroInstanceId;
    }
}
