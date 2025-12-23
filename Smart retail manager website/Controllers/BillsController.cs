using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Controllers
{
    public class BillController : Controller
    {
        private readonly AppDbContext _context;

        public BillController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bill (with search by customer name)
        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.Bill
                .Include(b => b.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(b => b.Customer.Name.Contains(q));
            }

            // 🔴 View: /Views/Bills/Index.cshtml
            return View("~/Views/Bills/Index.cshtml",
                        await query.OrderByDescending(b => b.Date).ToListAsync());
        }

        // GET: Bill/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.BillID == id);
            if (bill == null)
            {
                return NotFound();
            }

            // 🔴 View: /Views/Bills/Details.cshtml
            return View("~/Views/Bills/Details.cshtml", bill);
        }

        // GET: Bill/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name");

            // 🔴 View: /Views/Bills/Create.cshtml
            return View("~/Views/Bills/Create.cshtml");
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillID,Date,CustomerID,TaxRate")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name", bill.CustomerID);

            // 🔴 View: /Views/Bills/Create.cshtml
            return View("~/Views/Bills/Create.cshtml", bill);
        }

        // GET: Bill/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name", bill.CustomerID);

            // 🔴 View: /Views/Bills/Edit.cshtml
            return View("~/Views/Bills/Edit.cshtml", bill);
        }

        // POST: Bill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillID,Date,CustomerID,TaxRate")] Bill bill)
        {
            if (id != bill.BillID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name", bill.CustomerID);

            // 🔴 View: /Views/Bills/Edit.cshtml
            return View("~/Views/Bills/Edit.cshtml", bill);
        }

        // GET: Bill/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.BillID == id);
            if (bill == null)
            {
                return NotFound();
            }

            // 🔴 View: /Views/Bills/Delete.cshtml
            return View("~/Views/Bills/Delete.cshtml", bill);
        }

        // POST: Bill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill != null)
            {
                _context.Bill.Remove(bill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.BillID == id);
        }
    }
}
