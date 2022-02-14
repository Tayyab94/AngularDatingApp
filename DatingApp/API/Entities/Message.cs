using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int  SenderId { get; set; }

        public string SenderName { get; set; }

        public AppUser Sender { get; set; }

        
        public int  ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        public AppUser Receiver { get; set; }

        
        public string Content {get;set;}

        public DateTime? DataRead {get;set;}

        public DateTime MessageSent { get; set; } = DateTime.Now;
        
        public bool SenderDelated { get; set; }

        public bool ReceriverDeleted {get;set;}
    }
}