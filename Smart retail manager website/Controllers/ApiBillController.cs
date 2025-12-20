using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBillController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiBillController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiBill?q=ali
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBill([FromQuery] string? q)
        {
            var query = _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Bill_Products)
                    .ThenInclude(bp => bp.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(b => b.Customer.Name.Contains(q));
            }

            return await query.OrderByDescending(b => b.Date).ToListAsync();
        }


        // GET: api/ApiBill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var bill = await _context.Bill
            .Include(b => b.Customer)
            .Include(b => b.Bill_Products)
                .ThenInclude(bp => bp.Product)
            .FirstOrDefaultAsync(b => b.BillID == id);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;

        }

        // PUT: api/ApiBill/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, Bill dto)
        {
            if (id != dto.BillID)
                return BadRequest();

            // Load the existing Bill from DB (tracked entity)
            var bill = await _context.Bill.FirstOrDefaultAsync(b => b.BillID == id);
            if (bill == null)
                return NotFound();

            // Update ONLY scalar columns
            bill.Date = dto.Date;
            bill.CustomerID = dto.CustomerID;
            bill.TaxRate = dto.TaxRate;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/ApiBill
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
            _context.Bill.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBill", new { id = bill.BillID }, bill);
        }

        // DELETE: api/ApiBill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = await _context.Bill
            .Include(b => b.Bill_Products)
            .FirstOrDefaultAsync(b => b.BillID == id);

            if (bill == null)
            {
                return NotFound();
            }

            _context.Bill_Products.RemoveRange(bill.Bill_Products);
            _context.Bill.Remove(bill);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.BillID == id);
        }
    }
}
