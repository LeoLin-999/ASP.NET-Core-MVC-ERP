using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcERPTest01.Models;

namespace MvcERPTest01.Controllers
{
    public class Sales_DetailController : BaseController
    {
        private readonly ErpDbContext _context;

        public Sales_DetailController(ErpDbContext context)
        {
            _context = context;
        }

        // GET: Sales_Detail
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales_Detail.ToListAsync());
        }

        // GET: Sales_Detail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales_Detail = await _context.Sales_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (sales_Detail == null)
            {
                return NotFound();
            }

            return View(sales_Detail);
        }

        // GET: Sales_Detail/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales_Detail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Create_Date,Creater,Update_Date,Updater,SequenceID,Sales_ID,Product_ID,Sales_Qty,Sales_Amount,Sales_SubTotal")] Sales_Detail sales_Detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sales_Detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sales_Detail);
        }

        // GET: Sales_Detail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales_Detail = await _context.Sales_Detail.FindAsync(id);
            if (sales_Detail == null)
            {
                return NotFound();
            }
            return View(sales_Detail);
        }

        // POST: Sales_Detail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Create_Date,Creater,Update_Date,Updater,SequenceID,Sales_ID,Product_ID,Sales_Qty,Sales_Amount,Sales_SubTotal")] Sales_Detail sales_Detail)
        {
            if (id != sales_Detail.SequenceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sales_Detail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sales_DetailExists(sales_Detail.SequenceID))
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
            return View(sales_Detail);
        }

        // GET: Sales_Detail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales_Detail = await _context.Sales_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (sales_Detail == null)
            {
                return NotFound();
            }

            return View(sales_Detail);
        }

        // POST: Sales_Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sales_Detail = await _context.Sales_Detail.FindAsync(id);
            if (sales_Detail != null)
            {
                _context.Sales_Detail.Remove(sales_Detail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sales_DetailExists(int id)
        {
            return _context.Sales_Detail.Any(e => e.SequenceID == id);
        }
    }
}
