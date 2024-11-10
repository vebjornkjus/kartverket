using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Brukernavn er påkrevd.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
