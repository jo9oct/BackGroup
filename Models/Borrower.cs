using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Borrower
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(50)]
        public string MembershipId { get; set; } = string.Empty; // Optional unique membership ID

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}