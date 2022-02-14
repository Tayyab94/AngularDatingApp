namespace API.DTOs
{
    public class MessageDTO
    {
            public int Id { get; set; }

        public int  SenderId { get; set; }

        public string SenderName { get; set; }

        public string SenderPhotoUrl { get; set; }

        
        public int  ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhotoUrl { get; set; }

        
        public string Content {get;set;}

        public DateTime? DataRead {get;set;}

        public DateTime MessageSent { get; set; }
        
    
    }
}