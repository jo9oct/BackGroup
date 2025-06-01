using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using LibraryWebAPI.Dtos; // Your DTOs namespace
using System.Linq;

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protect all borrower endpoints
    public class BorrowersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/borrowers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowerDto>>> GetBorrowers()
        {
            return await _context.Borrowers
                .Select(b => new BorrowerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Email = b.Email,
                    MembershipId = b.MembershipId
                })
                .ToListAsync();
        }

        // GET: api/borrowers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowerDto>> GetBorrower(int id)
        {
            var borrower = await _context.Borrowers
                .Select(b => new BorrowerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Email = b.Email,
                    MembershipId = b.MembershipId
                })
                .FirstOrDefaultAsync(b => b.Id == id);


            if (borrower == null)
            {
                return NotFound();
            }

            return borrower;
        }

        // POST: api/borrowers
        [HttpPost]
        public async Task<ActionResult<BorrowerDto>> PostBorrower(CreateBorrowerDto createBorrowerDto)
        {
             if (await _context.Borrowers.AnyAsync(b => b.Email == createBorrowerDto.Email))
            {
                return BadRequest("A borrower with this email already exists.");
            }
            if (!string.IsNullOrEmpty(createBorrowerDto.MembershipId) && await _context.Borrowers.AnyAsync(b => b.MembershipId == createBorrowerDto.MembershipId))
            {
                return BadRequest("A borrower with this membership ID already exists.");
            }

            var borrower = new Borrower
            {
                Name = createBorrowerDto.Name,
                Email = createBorrowerDto.Email,
                MembershipId = createBorrowerDto.MembershipId
            };

            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();

            var borrowerDto = new BorrowerDto {
                Id = borrower.Id,
                Name = borrower.Name,
                Email = borrower.Email,
                MembershipId = borrower.MembershipId
            };

            return CreatedAtAction(nameof(GetBorrower), new { id = borrower.Id }, borrowerDto);
        }

        // PUT: api/borrowers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrower(int id, UpdateBorrowerDto updateBorrowerDto)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound("Borrower not found.");
            }

            // Check for email uniqueness if it's being changed
            if (borrower.Email != updateBorrowerDto.Email && await _context.Borrowers.AnyAsync(b => b.Email == updateBorrowerDto.Email && b.Id != id))
            {
                return BadRequest("Another borrower with this email already exists.");
            }
            // Check for MembershipId uniqueness if it's being changed and is not empty
            if (!string.IsNullOrEmpty(updateBorrowerDto.MembershipId) && borrower.MembershipId != updateBorrowerDto.MembershipId && await _context.Borrowers.AnyAsync(b => b.MembershipId == updateBorrowerDto.MembershipId && b.Id != id))
            {
                return BadRequest("Another borrower with this membership ID already exists.");
            }


            borrower.Name = updateBorrowerDto.Name;
            borrower.Email = updateBorrowerDto.Email;
            borrower.MembershipId = updateBorrowerDto.MembershipId;

            _context.Entry(borrower).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowerExists(id))
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

        // DELETE: api/borrowers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            // Optional: Check if there are active loans for this borrower
             if (await _context.Loans.AnyAsync(l => l.BorrowerId == id && l.ReturnDate == null))
            {
                return BadRequest("Cannot delete borrower. This borrower has active loans.");
            }


            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowerExists(int id)
        {
            return _context.Borrowers.Any(e => e.Id == id);
        }
    }
}