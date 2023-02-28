using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CollectionKeeper.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "NameRequired")]
        [StringLength(20, ErrorMessage = "NameLength", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
