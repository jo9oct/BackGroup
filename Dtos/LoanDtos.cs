using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Dtos
{
    public class IssueLoanDto
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public int BorrowerId { get; set; }
        // DueDate will be calculated by the server
    }

    public class ReturnLoanDto
    {
        [Required]
        public int LoanId { get; set; } // Or BookId if preferred, but LoanId is more specific
    }
     public class ReturnLoanByBookDto // Alternative way to return a book
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public int BorrowerId { get; set; } // To identify specific loan if multiple copies are loaned to same person
    }


    public class LoanDetailsDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}