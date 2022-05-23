using Armageddon.Backend.Payloads;

namespace Armageddon.Mechanics.Maps
{
    public class Stage
    {
        public Stage(StagePayload obj)
        {
            Payload = obj;
        }

        public int Id => Payload.Id;

        // You can call this an exception...
        public StagePayload Payload { get; }
    }
}
