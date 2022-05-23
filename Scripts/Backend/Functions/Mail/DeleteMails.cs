using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Functions
{
    [Exchange(AddConvertExtension = false)]
    public class DeleteMailsRequestEntry
    {
        public string InstanceId;
    }

    [FunctionRequest]
    public class DeleteMailsRequest : BackendRequest
    {
        public DeleteMailsRequestEntry[] Entries;
    }

    [FunctionReply]
    public class DeleteMailsReply : BackendReply
    {
        public string[] RemovedMailInstanceIds;
    }
}
