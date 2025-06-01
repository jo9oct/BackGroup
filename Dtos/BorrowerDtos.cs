using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Dtos
{
    public class BorrowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MembershipId { get; set; } = string.Empty;
    }

    public class CreateBorrowerDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(50)]
        public string MembershipId { get; set; } = string.Empty;
    }

     public class UpdateBorrowerDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(50)]
        public string MembershipId { get; set; } = string.Empty;
    }
}