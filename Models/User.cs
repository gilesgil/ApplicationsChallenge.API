using System.ComponentModel.DataAnnotations;

namespace ApplicationsChallenge.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        
        // Salt for password hashing
        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; } = string.Empty;
    }
}
