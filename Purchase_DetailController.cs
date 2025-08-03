using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcERPTest01.Models;

namespace MvcERPTest01.Controllers
{
	// 採購明細控制器
    public class Purchase_DetailController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public Purchase_DetailController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Purchase_Detail
        public async Task<IActionResult> Index()
        {
            return View(await _context.Purchase_Detail.ToListAsync());
        }

        // GET: Purchase_Detail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase_Detail = await _context.Purchase_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (purchase_Detail == null)
            {
                return NotFound();
            }

            return View(purchase_Detail);
        }

        // GET: Purchase_Detail/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Purchase_Detail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Create_Date,Creater,Update_Date,Updater,SequenceID,Purchase_ID,Product_ID,Purchase_Qty,Purchase_Amount,Purchase_SubTotal")] Purchase_Detail purchase_Detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchase_Detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(purchase_Detail);
        }

        // GET: Purchase_Detail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase_Detail = await _context.Purchase_Detail.FindAsync(id);
            if (purchase_Detail == null)
            {
                return NotFound();
            }

			// 下拉選單產品清單
			ViewBag.ProductList = new SelectList(
				_context.Products.Where(p => p.Product_Status == "1"),
				"Product_ID", "Product_Name");

            return View(purchase_Detail);
        }

        // POST: Purchase_Detail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SequenceID, string Purchase_ID)
        {
			string prodId = Request.Form["Product_ID"];
			string qty = Request.Form["Purchase_Qty"];
			
			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					await conn.OpenAsync();
					string sql = @"
						DECLARE @PurchaseID varchar(20), @ProductID varchar(20), @Qty int
						SELECT @PurchaseID = @sPurchaseID, @ProductID = @sProductID, @Qty = @sQty

						UPDATE Purchase_Detail
						SET Purchase_Qty = @Qty,
							Purchase_Amount = (SELECT Price FROM Products WITH (NOLOCK) WHERE Product_ID = @sProductID),
							Purchase_SubTotal = ((SELECT Price FROM Products WITH (NOLOCK) WHERE Product_ID = @sProductID) * @Qty),
							Update_Date = GETDATE(),
							Updater = @Updater
						WHERE SequenceID = @ID AND Purchase_ID = @PurchaseID

						UPDATE Purchase SET
							Total_Amount = (SELECT SUM(Purchase_SubTotal) FROM Purchase_Detail WITH (NOLOCK) WHERE Purchase_ID = @PurchaseID)
						WHERE Purchase_ID = @PurchaseID";
					using SqlCommand cmd = new SqlCommand(sql, conn);
					cmd.Parameters.AddWithValue("@sQty", qty);
					cmd.Parameters.AddWithValue("@Updater", UserID ?? "System");
					cmd.Parameters.AddWithValue("@ID", SequenceID);
					cmd.Parameters.AddWithValue("@sProductID", prodId);
					cmd.Parameters.AddWithValue("@sPurchaseID", Purchase_ID);

					await cmd.ExecuteNonQueryAsync();
				}

				TempData["Message"] = "採購明細已更新";
				return RedirectToAction("Edit", "Purchase", new { id = Purchase_ID });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"更新失敗：{ex.Message}");
				return View(); // 或導回錯誤頁
			}
        }

        // GET: Purchase_Detail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase_Detail = await _context.Purchase_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (purchase_Detail == null)
            {
                return NotFound();
            }

            return View(purchase_Detail);
        }

        // POST: Purchase_Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase_Detail = await _context.Purchase_Detail.FindAsync(id);
            if (purchase_Detail != null)
            {
                _context.Purchase_Detail.Remove(purchase_Detail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Purchase_DetailExists(int id)
        {
            return _context.Purchase_Detail.Any(e => e.SequenceID == id);
        }
    }
}
