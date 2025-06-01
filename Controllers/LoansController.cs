using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using LibraryWebAPI.Dtos;
using System;
using System.Linq; // Required for .Select, .Include

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protect all loan endpoints
    public class LoansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int DefaultLoanPeriodDays = 14; // e.g., 2 weeks

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/loans/issue (Changed route for clarity from just /api/loans)
        [HttpPost("issue")]
        public async Task<ActionResult<LoanDetailsDto>> IssueBook(IssueLoanDto issueLoanDto)
        {
            var book = await _context.Books.FindAsync(issueLoanDto.BookId);
            if (book == null)
            {
                return NotFound(new { Message = "Book not found." });
            }

            if (book.AvailableCopies <= 0)
            {
                return BadRequest(new { Message = "Book is not available." });
            }

            var borrower = await _context.Borrowers.FindAsync(issueLoanDto.BorrowerId);
            if (borrower == null)
            {
                return NotFound(new { Message = "Borrower not found." });
            }

            // Check if this borrower already has an active loan for this specific book (if library policy restricts this)
            // For simplicity, we'll allow it, but in a real system you might check.

            var loan = new Loan
            {
                BookId = issueLoanDto.BookId,
                BorrowerId = issueLoanDto.BorrowerId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(DefaultLoanPeriodDays),
                ReturnDate = null
            };

            book.AvailableCopies--;
            _context.Loans.Add(loan);

            await _context.SaveChangesAsync();

            return Ok(new LoanDetailsDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = book.Title,
                BorrowerId = loan.BorrowerId,
                BorrowerName = borrower.Name,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate
            });
        }

        // POST: api/loans/return (Changed from /api/returns to be under loans controller)
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook(ReturnLoanDto returnLoanDto)
        {
            var loan = await _context.Loans
                                .Include(l => l.Book) // Include the book to update its AvailableCopies
                                .FirstOrDefaultAsync(l => l.Id == returnLoanDto.LoanId && l.ReturnDate == null);

            if (loan == null)
            {
                return NotFound(new { Message = "Active loan not found or already returned." });
            }

            loan.ReturnDate = DateTime.UtcNow;
            if (loan.Book != null) // Should always be true if FK is set up
            {
                loan.Book.AvailableCopies++;
            }
            else
            {
                // Handle case where book might have been deleted - though FK should prevent this
                // Or log an error
                return StatusCode(500, "Associated book not found for the loan.");
            }


            await _context.SaveChangesAsync();
            return Ok(new { Message = "Book returned successfully." });
        }

        // Alternative POST: api/loans/returnbybook
        [HttpPost("returnbybook")]
        public async Task<IActionResult> ReturnBookByBookAndBorrower(ReturnLoanByBookDto dto)
        {
            var loan = await _context.Loans
                                .Include(l => l.Book)
                                .FirstOrDefaultAsync(l => l.BookId == dto.BookId && 
                                                        l.BorrowerId == dto.BorrowerId && 
                                                        l.ReturnDate == null);
            if (loan == null)
            {
                return NotFound(new { Message = "Active loan not found for this book and borrower, or already returned." });
            }

            loan.ReturnDate = DateTime.UtcNow;
            if (loan.Book != null)
            {
                loan.Book.AvailableCopies++;
            }
            else
            {
                 return StatusCode(500, "Associated book not found for the loan.");
            }
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Book returned successfully using book and borrower ID." });
        }


        // GET: api/loans/overdue
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDetailsDto>>> GetOverdueLoans()
        {
            var overdueLoans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .Where(l => l.ReturnDate == null && l.DueDate < DateTime.UtcNow)
                .Select(l => new LoanDetailsDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : "N/A",
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower != null ? l.Borrower.Name : "N/A",
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();

            return Ok(overdueLoans);
        }

        // GET: api/loans (List all loans - for admin purposes, might need pagination)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDetailsDto>>> GetAllLoans()
        {
             return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .OrderByDescending(l => l.LoanDate)
                .Select(l => new LoanDetailsDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : "N/A",
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower != null ? l.Borrower.Name : "N/A",
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();
        }

         // GET: api/loans/borrower/{borrowerId} (List all loans for a specific borrower)
        [HttpGet("borrower/{borrowerId}")]
        public async Task<ActionResult<IEnumerable<LoanDetailsDto>>> GetLoansByBorrower(int borrowerId)
        {
            if (!await _context.Borrowers.AnyAsync(b => b.Id == borrowerId))
            {
                return NotFound($"Borrower with ID {borrowerId} not found.");
            }

            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .Where(l => l.BorrowerId == borrowerId)
                .OrderByDescending(l => l.LoanDate)
                .Select(l => new LoanDetailsDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : "N/A",
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower != null ? l.Borrower.Name : "N/A",
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();
        }
    }
}