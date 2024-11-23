using Org.BouncyCastle.Cms;

namespace KartverketWebApp.Models
{

        public class SammtaleModel
        {
            public int RapportId { get; set; }
            public string Tittel { get; set; }
            public string LastMessage { get; set; }
            public string LastSenderName { get; set; }
            public string SenderName { get; set; }
            public string Status { get; set; }
            public int RecipientId { get; set; }

    }

}
