using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "{0} is required!!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "{0} must be beetwent {2} and {1} characters.")]
        public string Password { get; set; }
    }
}
