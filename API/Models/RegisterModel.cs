using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "{0} is required!!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "{0} must be beetwent {2} and {1} characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        public string KnownAs { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        public string Country { get; set; }

        [Required(ErrorMessage = "{0} is required!!")]
        public DateTime DateOfBirth { get; set; }
    }
}