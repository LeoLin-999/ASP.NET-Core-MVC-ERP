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
	// 採購控制器
    public class PurchaseController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public PurchaseController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Purchase
        public async Task<IActionResult> Index()
        {
			var purchaseDto = await (from p in _context.Purchase
				join s in _context.Suppliers on p.Supplier_ID equals s.Supplier_ID into supplierGroup
				from s in supplierGroup.DefaultIfEmpty()
				join w in _context.Warehouse on p.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new PurchaseDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Purchase_ID = p.Purchase_ID,
				 Supplier_ID = p.Supplier_ID,
				 Supplier_Name = s.Supplier_Name,
				 Warehouse_ID = p.Warehouse_ID,
				 Warehouse_Name = w.Warehouse_Name,
				 Purchase_Date = p.Purchase_Date,
				 Purchase_Status = p.Purchase_Status,
				 Total_Amount = p.Total_Amount,
				 Arrival_Date = p.Arrival_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).ToListAsync();
            return View(purchaseDto);
        }

        // GET: Purchase/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var purchaseDto = await (from p in _context.Purchase
				where p.Purchase_ID == id
				join s in _context.Suppliers on p.Supplier_ID equals s.Supplier_ID into supplierGroup
				from s in supplierGroup.DefaultIfEmpty()
				join w in _context.Warehouse on p.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new PurchaseDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Purchase_ID = p.Purchase_ID,
				 Supplier_ID = p.Supplier_ID,
				 Supplier_Name = s.Supplier_Name,
				 Warehouse_ID = p.Warehouse_ID,
				 Warehouse_Name = w.Warehouse_Name,
				 Purchase_Date = p.Purchase_Date,
				 Purchase_Status = p.Purchase_Status,
				 Total_Amount = p.Total_Amount,
				 Arrival_Date = p.Arrival_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).FirstOrDefaultAsync();

			// 採購單明細列表
			//ViewBag.DetailList = _context.Purchase_Detail
			//	.Where(p => p.Purchase_ID == id).ToList();
			var viewModel = new PurchaseDetailViewModel
			{
				// 採購主檔資訊
				PurchaseInfo = purchaseDto,
				// 採購單明細列表
				DetailList = _context.Purchase_Detail
					.Where(d => d.Purchase_ID == id).ToList()
			};

            return View(viewModel);
        }

        // GET: Purchase/Create
        public IActionResult Create()
        {
			// 供應商下拉選單
			ViewBag.SupplierList = new SelectList(_context.Suppliers, "Supplier_ID", "Supplier_Name");
			// 倉庫下拉選單
			ViewBag.WarehouseList = new SelectList(_context.Warehouse, "Warehouse_ID", "Warehouse_Name");

			// 採購單狀態
			var purchase = new Purchase
			{
				Purchase_Status = "0" // 預設為「初始」
			};

            return View(purchase);
        }

        // POST: Purchase/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Supplier_ID,Warehouse_ID,Purchase_Date,Purchase_Status,Arrival_Date")] Purchase purchase)
        {
			// 在這裡由伺服器端設定建立者與建立時間
			purchase.Create_Date = DateTime.Now;
			purchase.Creater = UserID ?? "System";
			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			if (ModelState.ContainsKey("Purchase_ID"))
				ModelState.Remove("Purchase_ID");
			// 除錯訊息
			if (!ModelState.IsValid)
			{
				foreach (var modelState in ModelState)
				{
					foreach (var error in modelState.Value.Errors)
					{
						Console.WriteLine($"欄位: {modelState.Key}, 錯誤: {error.ErrorMessage}");
					}
				}
			}

			// 產生 Purchase_ID：P + yyyyMM + 流水號
			string prefix = "P" + DateTime.Now.ToString("yyyyMM");
			var maxId = _context.Purchase
				.Where(p => p.Purchase_ID.StartsWith(prefix))
				.OrderByDescending(p => p.Purchase_ID)
				.Select(p => p.Purchase_ID)
				.FirstOrDefault();
			int newSeq = 1;
			if (!string.IsNullOrEmpty(maxId) && maxId.Length >= 12)
			{
				if (int.TryParse(maxId.Substring(7, 5), out int lastSeq))
				{
					newSeq = lastSeq + 1;
				}
			}
			purchase.Purchase_ID = prefix + newSeq.ToString("D5");

            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
				// 導向明細頁建立採購明細
				//return RedirectToAction("Details", new { id = purchase.Purchase_ID });
				// 導向編輯頁建立採購明細
				return RedirectToAction("Edit", new { id = purchase.Purchase_ID });
            }
            return View(purchase);
        }

        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
			if (string.IsNullOrEmpty(id)) return NotFound();

			// 取得採購主檔（原始資料）
			var purchase = await _context.Purchase.FindAsync(id);
			if (purchase == null) return NotFound();

			// 建立 ViewModel
			var viewModel = new PurchaseDetailViewModel
			{
				// 採購主檔資訊
				PurchaseForm = purchase,
				// 採購單明細表單
				DetailForm = new Purchase_Detail { Purchase_ID = id },
				// 採購單明細列表
				DetailList = _context.Purchase_Detail
					.Where(d => d.Purchase_ID == id).ToList()
			};
			// 供應商下拉選單（主表單）
			ViewBag.SupplierList = new SelectList(_context.Suppliers, "Supplier_ID", "Supplier_Name");
			// 倉庫下拉選單（主表單）
			ViewBag.WarehouseList = new SelectList(_context.Warehouse, "Warehouse_ID", "Warehouse_Name");

			// 下拉選單產品清單（若要新增明細）
			ViewBag.ProductList = new SelectList(
				_context.Products.Where(p => p.Product_Status == "1"),
				"Product_ID", "Product_Name");

			return View(viewModel);
        }

        // POST: Purchase/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Create_Date,Creater,Update_Date,Updater,Purchase_ID,Supplier_ID,Purchase_Date,Purchase_Status,Total_Amount,Arrival_Date")] Purchase purchase)
        {
            if (id != purchase.Purchase_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Purchase_ID))
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
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase
                .FirstOrDefaultAsync(m => m.Purchase_ID == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var purchase = await _context.Purchase.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchase.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(string id)
        {
            return _context.Purchase.Any(e => e.Purchase_ID == id);
        }

		// 編輯到貨
		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> ArrivalConfirm(PurchaseDetailViewModel vm)
		{
			var purchase = await _context.Purchase.FindAsync(vm.PurchaseForm.Purchase_ID);
			if (purchase == null) return NotFound();

			try
			{
				// 取得資料庫連線字串
				var connectionString = _configuration.GetConnectionString("ErpDbContext");
				using (SqlConnection conn = new SqlConnection(connectionString))
				{
					await conn.OpenAsync();

					// 更新採購主檔
					using (SqlCommand cmd = conn.CreateCommand())
					{
						cmd.CommandText = @"
							DECLARE @Purchase_ID varchar(20)
							SELECT @Purchase_ID = @id

							UPDATE Purchase
							SET Purchase_Status = @status,
								Purchase_Date = @date,
								Total_Amount = (SELECT SUM(Purchase_SubTotal) FROM Purchase_Detail WITH (NOLOCK) WHERE Purchase_ID = @Purchase_ID),
								Update_Date = @updateDate,
								Updater = @updater
							WHERE Purchase_ID = @Purchase_ID";

						cmd.Parameters.AddWithValue("@status", vm.PurchaseForm.Purchase_Status ?? (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@date", vm.PurchaseForm.Purchase_Date ?? DateTime.Now);
						cmd.Parameters.AddWithValue("@updateDate", DateTime.Now);
						cmd.Parameters.AddWithValue("@updater", UserID ?? "System");
						cmd.Parameters.AddWithValue("@id", vm.PurchaseForm.Purchase_ID);

						await cmd.ExecuteNonQueryAsync();
					}

					string warehouseId = purchase.Warehouse_ID ?? "WH01";
					var details = _context.Purchase_Detail
						.Where(d => d.Purchase_ID == purchase.Purchase_ID).ToList();
					foreach (var item in details)
					{
						int qty = item.Purchase_Qty ?? 0;
						string productId = item.Product_ID!;

						// 先查詢現有庫存
						string selectSql = @"
							SELECT Quantity, MaxStockLevel 
							FROM Inventory
							WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID";

						using (SqlCommand selectCmd = new SqlCommand(selectSql, conn))
						{
							selectCmd.Parameters.AddWithValue("@ProductID", productId);
							selectCmd.Parameters.AddWithValue("@WarehouseID", warehouseId);

							using var reader = await selectCmd.ExecuteReaderAsync();
							bool exists = reader.HasRows;
							int currentQty = 0;
							int? maxLevel = null;
							if (exists)
							{
								while (await reader.ReadAsync())
								{
									if (!reader.IsDBNull(0))
										currentQty = reader.GetInt32(0);
									if (!reader.IsDBNull(1))
										maxLevel = reader.GetInt32(1);
								}
							}
							await reader.CloseAsync();

							if (exists)
							{
								// Update 庫存
								string notes = "";
								int newQty = currentQty + qty;
								if (maxLevel.HasValue && newQty > maxLevel)
								{
									notes = $"庫存超過上限！目前數量 {newQty}，最大值為 {maxLevel}";
								}

								string updateSql = @"
									UPDATE Inventory
									SET Quantity = @NewQty,
										LastStockInDate = GETDATE(),
										Update_Date = GETDATE(),
										Updater = @Updater,
										Notes = @Notes
									WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID";

								using SqlCommand updateCmd = new SqlCommand(updateSql, conn);
								updateCmd.Parameters.AddWithValue("@NewQty", newQty);
								updateCmd.Parameters.AddWithValue("@Updater", UserID ?? "System");
								updateCmd.Parameters.AddWithValue("@Notes", notes);
								updateCmd.Parameters.AddWithValue("@ProductID", productId);
								updateCmd.Parameters.AddWithValue("@WarehouseID", warehouseId);
								await updateCmd.ExecuteNonQueryAsync();
							}
							else
							{
								// Insert 新庫存
								string insertSql = @"
									INSERT INTO Inventory (
										Inventory_ID, Product_ID, Warehouse_ID,
										Quantity, LastStockInDate, Create_Date, Creater
									) SELECT
										RIGHT(REPLICATE('0', 8) + CAST((COUNT(Inventory_ID) + 1) as varchar), 8)
										, @ProductID, @WarehouseID,
										@Qty, GETDATE(), GETDATE(), @Creater
										FROM Inventory WITH (NOLOCK)
										WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID
										
									";

								using SqlCommand insertCmd = new SqlCommand(insertSql, conn);
								insertCmd.Parameters.AddWithValue("@ProductID", productId);
								insertCmd.Parameters.AddWithValue("@WarehouseID", warehouseId);
								insertCmd.Parameters.AddWithValue("@Qty", qty);
								insertCmd.Parameters.AddWithValue("@Creater", UserID ?? "System");
								await insertCmd.ExecuteNonQueryAsync();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "更新資料時發生錯誤：" + ex.Message);
				return RedirectToAction("Edit", new { id = purchase.Purchase_ID });
			}

			//return RedirectToAction("Edit", new { id = purchase.Purchase_ID });
			return RedirectToAction(nameof(Index));
		}

		// [HttpPost] 新增明細資料
		[HttpPost]
		public async Task<IActionResult> AddDetail([Bind(Prefix = "DetailForm")] Purchase_Detail detail)
		{
			// 取得產品價格
			var product = await _context.Products
				.FirstOrDefaultAsync(p => p.Product_ID == detail.Product_ID);
			if (product == null || product.Price == null)
			{
				ModelState.AddModelError("DetailForm.Product_ID", "產品不存在或價格未設定");
			}

			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			detail.Create_Date = DateTime.Now;
			detail.Creater = UserID ?? "System";
			detail.Purchase_Amount = product!.Price;  // 帶入產品主檔價格
			detail.Purchase_SubTotal = (detail.Purchase_Qty ?? 0) * (detail.Purchase_Amount ?? 0);

			_context.Purchase_Detail.Add(detail);

			var purchase = await _context.Purchase.FindAsync(detail.Purchase_ID);
			if (purchase != null)
			{
				purchase.Total_Amount = (_context.Purchase_Detail
					.Where(p => p.Purchase_ID == detail.Purchase_ID)
					.Sum(p => p.Purchase_SubTotal) ?? 0) + detail.Purchase_SubTotal;

				purchase.Update_Date = DateTime.Now;
				purchase.Updater = UserID ?? "System";
				_context.Purchase.Update(purchase);
			}

			await _context.SaveChangesAsync();
			//return RedirectToAction("Details", new { id = detail.Purchase_ID });
			return RedirectToAction("Edit", new { id = detail.Purchase_ID });
		}

		// 執行刪除操作
		[HttpPost, ActionName("DeleteDetail")]
		public async Task<IActionResult> DeleteDetailConfirmed(int id)
		{
			var detail = await _context.Purchase_Detail.FindAsync(id);
			if (detail != null)
			{
				_context.Purchase_Detail.Remove(detail);

				var purchase = await _context.Purchase.FindAsync(detail.Purchase_ID);
				if (purchase != null && detail.Purchase_SubTotal.HasValue)
				{
					purchase.Total_Amount -= detail.Purchase_SubTotal;
					_context.Purchase.Update(purchase);
				}

				await _context.SaveChangesAsync();
			}

			//return RedirectToAction("Details", new { id = detail.Purchase_ID });
			return RedirectToAction("Edit", new { id = detail.Purchase_ID });
		}
    }
}
