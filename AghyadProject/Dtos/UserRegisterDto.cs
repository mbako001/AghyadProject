using System.ComponentModel.DataAnnotations;

namespace AghyadProject.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
