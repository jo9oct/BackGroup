using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)] // ISBN-13 or ISBN-10
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }
        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}