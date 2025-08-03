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
	// 庫存控制器
    public class InventoryController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public InventoryController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
			var inventoryDto = await (from p in _context.Inventory
				join s in _context.Products on p.Product_ID equals s.Product_ID into productsGroup
				from s in productsGroup.DefaultIfEmpty()
				join w in _context.Warehouse on p.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new InventoryDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Inventory_ID = p.Inventory_ID,
				 Product_ID = p.Product_ID,
				 Product_Name = s.Product_Name,
				 Warehouse_ID = p.Warehouse_ID,
				 Warehouse_Name = w.Warehouse_Name,
				 Inventory_Status = p.Inventory_Status,
				 Quantity = p.Quantity,
				 MinStockLevel = p.MinStockLevel,
				 LastStockInDate = p.LastStockInDate,
				 LastStockOutDate = p.LastStockOutDate,
				 ExpirationDate = p.ExpirationDate,
				 Notes = p.Notes,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).ToListAsync();
            return View(inventoryDto);
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var inventoryDto = await (from p in _context.Inventory
				where p.Inventory_ID == id
				join s in _context.Products on p.Product_ID equals s.Product_ID into productsGroup
				from s in productsGroup.DefaultIfEmpty()
				join w in _context.Warehouse on p.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new InventoryDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Inventory_ID = p.Inventory_ID,
				 Product_ID = p.Product_ID,
				 Product_Name = s.Product_Name,
				 Warehouse_ID = p.Warehouse_ID,
				 Warehouse_Name = w.Warehouse_Name,
				 Inventory_Status = p.Inventory_Status,
				 Quantity = p.Quantity,
				 MinStockLevel = p.MinStockLevel,
				 LastStockInDate = p.LastStockInDate,
				 LastStockOutDate = p.LastStockOutDate,
				 ExpirationDate = p.ExpirationDate,
				 Notes = p.Notes,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).FirstOrDefaultAsync();

            return View(inventoryDto);
        }

        // GET: Inventory/Create
        public IActionResult Create()
        {
			// 產品下拉選單
			ViewBag.ProductList = new SelectList(
				_context.Products.Where(p => p.Product_Status == "1"),
				"Product_ID", "Product_Name");

			// 倉庫下拉選單
			ViewBag.WarehouseList = new SelectList(_context.Warehouse, "Warehouse_ID", "Warehouse_Name");

			// 庫存狀態
			var inventory = new Inventory
			{
				Inventory_Status = "0" // 預設為「初始」
			};

            return View(inventory);
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Product_ID, string Warehouse_ID)
        {
			//string prodId = Request.Form["Product_ID"];
			//string whId = Request.Form["Warehouse_ID"];
			string iStatus = Request.Form["Inventory_Status"];
			string minStockLevel = Request.Form["MinStockLevel"];
			string maxStockLevel = Request.Form["MaxStockLevel"];
			string notes = Request.Form["Notes"];

			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					await conn.OpenAsync();
					string sql = @"
						DECLARE
						@Creater varchar(20)
						, @Product_ID varchar(20)
						, @Warehouse_ID varchar(20)
						, @Inventory_Status varchar(1)
						, @MinStockLevel int
						, @MaxStockLevel int
						, @Notes nvarchar(500)
						, @ProcessCount int = 0
						, @Result nvarchar(2000) = N''

						SELECT @Creater = @sCreater
							, @Product_ID = @sProduct_ID
							, @Warehouse_ID = @sWarehouse_ID
							, @Inventory_Status = @sInventory_Status
							, @MinStockLevel = @sMinStockLevel
							, @MaxStockLevel = @sMaxStockLevel
							, @Notes = @sNotes

						IF EXISTS (SELECT Inventory_ID FROM Inventory WITH (NOLOCK) WHERE Product_ID = @Product_ID AND Warehouse_ID = @Warehouse_ID AND Inventory_Status = N'1')
						BEGIN
						  SET @Result = N'NG@@產品代號[' + @Product_ID + N']儲存倉庫[' + @Warehouse_ID + N']已存在' + CHAR(13) + CHAR(10)
						END
						ELSE
						BEGIN
							INSERT INTO Inventory (
								Create_Date, Creater, Inventory_ID, Product_ID, Warehouse_ID, Inventory_Status, MinStockLevel, MaxStockLevel, Notes)
							SELECT GETDATE(), @Creater, N'I' + CONVERT(varchar(6), GETDATE(), 112) + RIGHT('00000000' + CONVERT(varchar(8), COUNT(Inventory_ID) + 1), 8)
								, @Product_ID, @Warehouse_ID, @Inventory_Status, @MinStockLevel, @MaxStockLevel, @Notes
							FROM (SELECT Inventory_ID, CONVERT(varchar(6), Create_Date, 112) AS [Create_Date] FROM Inventory WITH (NOLOCK)) AS [a]
							WHERE CONVERT(varchar(6), Create_Date, 112) = CONVERT(varchar(6), GETDATE(), 112)

							SELECT @ProcessCount = @@ROWCOUNT;
							IF @ProcessCount > 0
								SET @Result = N'OK@@產品代號[' + @Product_ID + N']儲存倉庫[' + @Warehouse_ID + N']完成' + CHAR(13) + CHAR(10)
							ELSE
								SET @Result = N'NG@@產品代號[' + @Product_ID + N']儲存倉庫[' + @Warehouse_ID + N']失敗' + CHAR(13) + CHAR(10)
						END

						SELECT @Result AS [Result]";
						using SqlCommand cmd = new SqlCommand(sql, conn);
						cmd.Parameters.Add(new SqlParameter("@sCreater", SqlDbType.VarChar, 20) { Value = UserID ?? "System" });
						cmd.Parameters.Add(new SqlParameter("@sProduct_ID", SqlDbType.VarChar, 20) { Value = Product_ID });
						cmd.Parameters.Add(new SqlParameter("@sWarehouse_ID", SqlDbType.VarChar, 20) { Value = Warehouse_ID });
						cmd.Parameters.Add(new SqlParameter("@sInventory_Status", SqlDbType.VarChar, 1) { Value = iStatus });
						cmd.Parameters.Add(new SqlParameter("@sMinStockLevel", SqlDbType.Int) { Value = minStockLevel });
						cmd.Parameters.Add(new SqlParameter("@sMaxStockLevel", SqlDbType.Int) { Value = maxStockLevel });
						cmd.Parameters.Add(new SqlParameter("@sNotes", SqlDbType.NVarChar, 500) { Value = UserID ?? "System" });
						string result = cmd.ExecuteScalar()?.ToString();
						if (result.StartsWith("NG@@"))
							ModelState.AddModelError("", $"新增失敗：{result}");
						else
							return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"新增失敗：{ex.Message}");
				return View(); // 或導回錯誤頁
			}
            return View("Index");
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(string id)
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

            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Inventory_ID, string Product_ID, string Warehouse_ID)
        {
			//string prodId = Request.Form["Product_ID"];
			//string whId = Request.Form["Warehouse_ID"];
			string iStatus = Request.Form["Inventory_Status"];
			string minStockLevel = Request.Form["MinStockLevel"];
			string maxStockLevel = Request.Form["MaxStockLevel"];
			string notes = Request.Form["Notes"];

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
						, @Inventory_ID varchar(20)
						, @Product_ID varchar(20)
						, @Warehouse_ID varchar(20)
						, @Inventory_Status varchar(1)
						, @MinStockLevel int
						, @MaxStockLevel int
						, @Notes nvarchar(500)
						, @ProcessCount int = 0
						, @Result nvarchar(2000) = N''

						SELECT @Updater = @sUpdater
							, @Inventory_ID = @sInventory_ID
							, @Product_ID = @sProduct_ID
							, @Warehouse_ID = @sWarehouse_ID
							, @Inventory_Status = @sInventory_Status
							, @MinStockLevel = @sMinStockLevel
							, @MaxStockLevel = @sMaxStockLevel
							, @Notes = @sNotes

						UPDATE Inventory SET
							Update_Date = GETDATE(), Updater = @Updater
							, Product_ID = @Product_ID, Warehouse_ID = @Warehouse_ID, Inventory_Status = @Inventory_Status
							, MinStockLevel = @MinStockLevel, MaxStockLevel = @MaxStockLevel, Notes = @Notes
						WHERE Inventory_ID = @Inventory_ID

						SELECT @ProcessCount = @@ROWCOUNT;
						IF @ProcessCount > 0
							SET @Result = N'OK@@產品代號[' + @Product_ID + N']儲存倉庫[' + @Warehouse_ID + N']完成' + CHAR(13) + CHAR(10)
						ELSE
							SET @Result = N'NG@@產品代號[' + @Product_ID + N']儲存倉庫[' + @Warehouse_ID + N']失敗' + CHAR(13) + CHAR(10)

						SELECT @Result AS [Result]";
						using SqlCommand cmd = new SqlCommand(sql, conn);
						cmd.Parameters.Add(new SqlParameter("@sUpdater", SqlDbType.VarChar, 20) { Value = UserID ?? "System" });
						cmd.Parameters.Add(new SqlParameter("@sProduct_ID", SqlDbType.VarChar, 20) { Value = Product_ID });
						cmd.Parameters.Add(new SqlParameter("@sWarehouse_ID", SqlDbType.VarChar, 20) { Value = Warehouse_ID });
						cmd.Parameters.Add(new SqlParameter("@sInventory_Status", SqlDbType.VarChar, 1) { Value = iStatus });
						cmd.Parameters.Add(new SqlParameter("@sMinStockLevel", SqlDbType.Int) { Value = minStockLevel });
						cmd.Parameters.Add(new SqlParameter("@sMaxStockLevel", SqlDbType.Int) { Value = maxStockLevel });
						cmd.Parameters.Add(new SqlParameter("@sNotes", SqlDbType.NVarChar, 500) { Value = UserID ?? "System" });
						cmd.Parameters.Add(new SqlParameter("@sInventory_ID", SqlDbType.VarChar, 20) { Value = Inventory_ID });
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

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.Inventory_ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventory.Remove(inventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(string id)
        {
            return _context.Inventory.Any(e => e.Inventory_ID == id);
        }
    }
}
