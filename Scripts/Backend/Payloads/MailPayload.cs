using System;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Mails;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public class MailPayload
    {
        public string InstanceId;
        public MailType Type;
        public string Title;
        public string Message;
        public DateTime ReceivedDateTime;
        public DateTime ExpiredDateTime;
        public MailAttachedItemPayload[] AttachedItems = { };
    }
}
