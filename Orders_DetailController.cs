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
	// 訂單明細控制器
    public class Orders_DetailController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public Orders_DetailController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Orders_Detail
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders_Detail.ToListAsync());
        }

        // GET: Orders_Detail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders_Detail = await _context.Orders_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (orders_Detail == null)
            {
                return NotFound();
            }

            return View(orders_Detail);
        }

        // GET: Orders_Detail/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders_Detail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Create_Date,Creater,Update_Date,Updater,SequenceID,Orders_ID,Product_ID,Orders_Qty,Orders_Amount,Orders_SubTotal")] Orders_Detail orders_Detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders_Detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders_Detail);
        }

        // GET: Orders_Detail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			// 產品下拉選單
			ViewBag.ProductList = new SelectList(
				_context.Products.Where(p => p.Product_Status == "1"),
				"Product_ID", "Product_Name");

			// 倉庫下拉選單
			ViewBag.WarehouseList = new SelectList(_context.Warehouse, "Warehouse_ID", "Warehouse_Name");

            var orders_Detail = await _context.Orders_Detail.FindAsync(id);
            if (orders_Detail == null)
            {
                return NotFound();
            }
            return View(orders_Detail);
        }

        // POST: Orders_Detail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SequenceID, string Orders_ID)
        {
			string prodId = Request.Form["Product_ID"];
			string whId = Request.Form["Warehouse_ID"];
			string qty = Request.Form["Orders_Qty"];
			
			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					await conn.OpenAsync();
					string sql = @"
						DECLARE @OrdersID varchar(20), @ProductID varchar(20), @Warehouse_ID varchar(20), @Qty int
						SELECT @OrdersID = @sOrdersID, @ProductID = @sProductID, @Warehouse_ID = @sWarehouse_ID, @Qty = @sQty

						UPDATE Orders_Detail
						SET Orders_Qty = @Qty,
							Orders_Amount = (SELECT Price FROM Products WITH (NOLOCK) WHERE Product_ID = @sProductID),
							Orders_SubTotal = ((SELECT Price FROM Products WITH (NOLOCK) WHERE Product_ID = @sProductID) * @Qty),
							Update_Date = GETDATE(),
							Updater = @Updater
						WHERE SequenceID = @ID AND Orders_ID = @OrdersID

						UPDATE Orders SET
							Total_Amount = (SELECT SUM(Orders_SubTotal) FROM Orders_Detail WITH (NOLOCK) WHERE Orders_ID = @OrdersID)
						WHERE Orders_ID = @OrdersID";
					using SqlCommand cmd = new SqlCommand(sql, conn);
					cmd.Parameters.AddWithValue("@sQty", qty);
					cmd.Parameters.AddWithValue("@Updater", UserID ?? "System");
					cmd.Parameters.AddWithValue("@ID", SequenceID);
					cmd.Parameters.AddWithValue("@sOrdersID", Orders_ID);
					cmd.Parameters.AddWithValue("@sProductID", prodId);
					cmd.Parameters.AddWithValue("@sWarehouse_ID", whId);

					await cmd.ExecuteNonQueryAsync();
				}

				TempData["Message"] = "訂單明細已更新";
				return RedirectToAction("Edit", "Orders", new { id = Orders_ID });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"更新失敗：{ex.Message}");
				return View(); // 或導回錯誤頁
			}
        }

        // GET: Orders_Detail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders_Detail = await _context.Orders_Detail
                .FirstOrDefaultAsync(m => m.SequenceID == id);
            if (orders_Detail == null)
            {
                return NotFound();
            }

            return View(orders_Detail);
        }

        // POST: Orders_Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders_Detail = await _context.Orders_Detail.FindAsync(id);
            if (orders_Detail != null)
            {
                _context.Orders_Detail.Remove(orders_Detail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Orders_DetailExists(int id)
        {
            return _context.Orders_Detail.Any(e => e.SequenceID == id);
        }
    }
}
