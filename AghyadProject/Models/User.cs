using System.ComponentModel.DataAnnotations;

namespace AghyadProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public double Coin { get; set; }
        public string FirstName { get; set; } = string.Empty;    
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime? RestTokenExpires { get; set; }
        public string? PasswordResetToken { get; set; }
    }
}
