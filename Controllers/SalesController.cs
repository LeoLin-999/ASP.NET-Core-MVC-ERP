using System;
using System.Collections.Generic;
using System.Data;
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
    public class SalesController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public SalesController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
			var salesDto = await (from p in _context.Sales
				join c in _context.Sys_Accounts on p.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on p.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new SalesDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Sales_ID = p.Sales_ID,
				 Orders_ID = p.Orders_ID,
				 Sales_Date = p.Sales_Date,
				 Sales_Status = p.Sales_Status,
				 Total_Amount = p.Total_Amount,
				 Shipping_Date = p.Shipping_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).ToListAsync();
            return View(salesDto);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			// 銷貨主檔
			var salesDto = await (from p in _context.Sales
				where p.Sales_ID == id
				join c in _context.Sys_Accounts on p.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on p.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new SalesDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Sales_ID = p.Sales_ID,
				 Orders_ID = p.Orders_ID,
				 Sales_Date = p.Sales_Date,
				 Sales_Status = p.Sales_Status,
				 Total_Amount = p.Total_Amount,
				 Shipping_Date = p.Shipping_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).FirstOrDefaultAsync();

			// 銷貨明細清單
			var detailList = await (from d in _context.Sales_Detail
				where d.Sales_ID == id
				join p in _context.Products on d.Product_ID equals p.Product_ID into productGroup
				from p in productGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on d.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on d.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Sales_DetailDto
				{
					SequenceID = d.SequenceID,
					Product_ID = d.Product_ID,
					Product_Name = p.Product_Name,
					Sales_Qty = d.Sales_Qty,
					Sales_Amount = d.Sales_Amount,
					Sales_SubTotal = d.Sales_SubTotal,
					Create_Date = d.Create_Date,
					Creater = d.Creater,
					CreaterName = c != null ? c.FullName : d.Creater,
					Update_Date = d.Update_Date,
					Updater = d.Updater,
					UpdaterName = u != null ? u.FullName : d.Updater
				}).ToListAsync();

			var viewModel = new SalesViewModel
			{
				// 銷貨主檔資訊
				SalesInfo = salesDto,
				// 銷貨明細清單
				DetailList = detailList
			};

            return View(viewModel);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Create_Date,Creater,Update_Date,Updater,Sales_ID,Sales_Date,Sales_Status,Total_Amount,Shipping_Date")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sales);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }

			// 銷貨明細清單
			var detailList = await (from d in _context.Sales_Detail
				where d.Sales_ID == id
				join p in _context.Products on d.Product_ID equals p.Product_ID into productGroup
				from p in productGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on d.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on d.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Sales_DetailDto
				{
					SequenceID = d.SequenceID,
					Product_ID = d.Product_ID,
					Product_Name = p.Product_Name,
					Sales_Qty = d.Sales_Qty,
					Sales_Amount = d.Sales_Amount,
					Sales_SubTotal = d.Sales_SubTotal,
					Create_Date = d.Create_Date,
					Creater = d.Creater,
					CreaterName = c != null ? c.FullName : d.Creater,
					Update_Date = d.Update_Date,
					Updater = d.Updater,
					UpdaterName = u != null ? u.FullName : d.Updater
				}).ToListAsync();

			var viewModel = new SalesViewModel
			{
				// 銷貨主檔表單
				SalesForm = sales,
				// 銷貨明細清單
				DetailList = detailList
			};
            return View(viewModel);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(Prefix = "SalesForm")] Sales sales)
        {
			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");
			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					await conn.OpenAsync();
					string sql = @"
						DECLARE
						@Updater varchar(20)
						, @Sales_ID varchar(20)
						, @Sales_Status varchar(1)
						, @ProcessCount int = 0
						, @Result nvarchar(2000) = N''

						SELECT @Updater = @sUpdater
							, @Sales_ID = @sSales_ID
							, @Sales_Status = @sSales_Status

						UPDATE Sales SET
							Update_Date = GETDATE(), Updater = @Updater
							, Sales_ID = @Sales_ID, Sales_Status = @Sales_Status
						WHERE Sales_ID = @Sales_ID

						SELECT @ProcessCount = @@ROWCOUNT;
						IF @ProcessCount > 0
							SET @Result = N'OK@@銷貨單號[' + @Sales_ID + N']完成' + CHAR(13) + CHAR(10)
						ELSE
							SET @Result = N'NG@@銷貨單號[' + @Sales_ID + N']失敗' + CHAR(13) + CHAR(10)

						SELECT @Result AS [Result]";
						using SqlCommand cmd = new SqlCommand(sql, conn);
						cmd.Parameters.Add(new SqlParameter("@sUpdater", SqlDbType.VarChar, 20) { Value = UserID ?? "System" });
						cmd.Parameters.Add(new SqlParameter("@sSales_ID", SqlDbType.VarChar, 20) { Value = sales.Sales_ID });
						cmd.Parameters.Add(new SqlParameter("@sSales_Status", SqlDbType.VarChar, 1) { Value = sales.Sales_Status });
						string result = cmd.ExecuteScalar()?.ToString();
						if (result.StartsWith("NG@@"))
							ModelState.AddModelError("", $"更新失敗：{result}");
						else
							return RedirectToAction(nameof(Index));
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"更新失敗：{ex.Message}");
				return View(); // 或導回錯誤頁
			}
            return View("Index");
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .FirstOrDefaultAsync(m => m.Sales_ID == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales != null)
            {
                _context.Sales.Remove(sales);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesExists(string id)
        {
            return _context.Sales.Any(e => e.Sales_ID == id);
        }
    }
}
