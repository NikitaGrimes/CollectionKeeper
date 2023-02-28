using System.ComponentModel.DataAnnotations;

namespace CollectionKeeper.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "NameRequired")]
        [StringLength(20, ErrorMessage = "NameLength", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress (ErrorMessage = "EmailAddress")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
