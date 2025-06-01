using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using LibraryWebAPI.Dtos;
using System.Linq; // Required for .Select

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protect all endpoints in this controller
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        [AllowAnonymous] // Example: Make listing books public
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            return await _context.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublishedYear = b.PublishedYear,
                    Genre = b.Genre,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies
                })
                .ToListAsync();
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Example: Make getting a single book public
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _context.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublishedYear = b.PublishedYear,
                    Genre = b.Genre,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies
                })
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto createBookDto)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == createBookDto.ISBN))
            {
                return BadRequest("A book with this ISBN already exists.");
            }

            var book = new Book
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                ISBN = createBookDto.ISBN,
                PublishedYear = createBookDto.PublishedYear,
                Genre = createBookDto.Genre,
                TotalCopies = createBookDto.TotalCopies,
                AvailableCopies = createBookDto.TotalCopies // Initially, all copies are available
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookDto = new BookDto // Map to DTO for response
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                Genre = book.Genre,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies
            };
  return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, UpdateBookDto updateBookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Check if TotalCopies is less than (TotalCopies - AvailableCopies) which is loaned out copies
            int loanedCopies = book.TotalCopies - book.AvailableCopies;
            if (updateBookDto.TotalCopies < loanedCopies) {
                return BadRequest($"Cannot set total copies to {updateBookDto.TotalCopies}. There are currently {loanedCopies} copies on loan.");
            }
            if (updateBookDto.AvailableCopies > updateBookDto.TotalCopies) {
                return BadRequest($"Available copies ({updateBookDto.AvailableCopies}) cannot exceed total copies ({updateBookDto.TotalCopies}).");
            }
             if (updateBookDto.AvailableCopies < 0) {
                return BadRequest($"Available copies ({updateBookDto.AvailableCopies}) cannot be negative.");
            }


            book.Title = updateBookDto.Title;
            book.Author = updateBookDto.Author;
            book.PublishedYear = updateBookDto.PublishedYear;
            book.Genre = updateBookDto.Genre;
            book.TotalCopies = updateBookDto.TotalCopies;
            book.AvailableCopies = updateBookDto.AvailableCopies;
            // Consider if ISBN should be updatable. If so, check for uniqueness again.

            _context.Entry(book).State = EntityState.Modified;

            try
            { await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
  return NoContent();
            }

            // DELETE: api/books/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteBook(int id)
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                // Optional: Check if there are active loans for this book
                if (await _context.Loans.AnyAsync(l => l.BookId == id && l.ReturnDate == null))
                {
                    return BadRequest("Cannot delete book. There are active loans for this book.");
                }


                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool BookExists(int id)
            {
                return _context.Books.Any(e => e.Id == id);
            }
        }
    }