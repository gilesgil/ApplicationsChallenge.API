using System.ComponentModel.DataAnnotations;

namespace ApplicationsChallenge.API.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // request, offer, complaint
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // submitted, completed
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        // Foreign key relationship
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
