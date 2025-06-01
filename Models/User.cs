using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        // public byte[] PasswordSalt { get; set; } // If using custom salt generation
    }
}