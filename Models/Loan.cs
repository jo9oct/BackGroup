 using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebAPI.Models
        {
            public class Loan
            {
                public int Id { get; set; }

                [Required]
                public int BookId { get; set; }
                [ForeignKey("BookId")]
                public Book? Book { get; set; }

                [Required]
                public int BorrowerId { get; set; }
                [ForeignKey("BorrowerId")]
                public Borrower? Borrower { get; set; }
                [Required]
                public DateTime LoanDate { get; set; }
                [Required]
                public DateTime DueDate { get; set; }
                public DateTime? ReturnDate { get; set; } // Nullable if not yet returned
            }
        }