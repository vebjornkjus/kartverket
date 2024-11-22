namespace KartverketWebApp.Models
{
    public class SammtaleModel
    {
        public int RapportId { get; set; }
        public string Tittel { get; set; }
        public string SenderName { get; set; } // Name of the person you're having a conversation with
        public string LastSenderName { get; set; } // "deg" or the sender's name
        public string LastMessage { get; set; } // Content of the last message
    }
}
