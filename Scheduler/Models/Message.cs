using System.Collections.Generic;
using System.Linq;

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
            return Data.data != null ? Data.data.Where(x => string.IsNullOrEmpty(x.Status) || x.Status != "sent").ToList()
                : new List<Message>();
        }
    }
}
