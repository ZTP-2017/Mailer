using System.Collections.Generic;

namespace Scheduler.Models
{
    interface IMessage
    {
        List<Message> GetData();
    }

    public class Message : IMessage
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }

        public List<Message> GetData()
        {
            return Data.data != null ? Data.data : new List<Message>();
        }
    }
}
