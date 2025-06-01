using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Dtos
{
    public class BookDto // For responses
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
    }

    public class CreateBookDto // For POST
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }
        // AvailableCopies will be set to TotalCopies initially
    }

    public class UpdateBookDto // For PUT
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
        // ISBN might not be updatable, or requires special handling
        public int PublishedYear { get; set; }
        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }
        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; } // Allow updating available copies directly if needed
    }
}