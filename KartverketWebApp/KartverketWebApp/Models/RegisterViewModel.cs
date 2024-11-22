using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Fornavn er påkrevd.")]
        [StringLength(50, ErrorMessage = "Fornavnet kan ikke være lengre enn {1} tegn.")]
        [Display(Name = "Fornavn")]
        public string Fornavn { get; set; }

        [Required(ErrorMessage = "Etternavn er påkrevd.")]
        [StringLength(50, ErrorMessage = "Etternavnet kan ikke være lengre enn {1} tegn.")]
        [Display(Name = "Etternavn")]
        public string Etternavn { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd.")]
        [EmailAddress(ErrorMessage = "E-posten må være i riktig format.")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Passordet må være minst {2} tegn langt.", MinimumLength = 8)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bekreft passord er påkrevd.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
        [Display(Name = "Bekreft passord")]
        public string ConfirmPassword { get; set; }
    }
}
