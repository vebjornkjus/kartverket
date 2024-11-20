using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Models
{
       public class RegisterViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Fornavn { get; set; }

            [Required]
            public string Etternavn { get; set; }
        }


}
