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
	// 訂單控制器
    public class OrdersController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public OrdersController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
			var ordersDto = await (from p in _context.Orders
				join s in _context.Customers on p.Customer_ID equals s.Customer_ID into customerGroup
				from s in customerGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on p.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on p.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new OrdersDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Orders_ID = p.Orders_ID,
				 Customer_ID = p.Customer_ID,
				 Customer_Name = s.Customer_Name,
				 Orders_Date = p.Orders_Date,
				 Orders_Status = p.Orders_Status,
				 Orders_Type = p.Orders_Type,
				 Total_Amount = p.Total_Amount,
				 Delivery_Date = p.Delivery_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).ToListAsync();
            return View(ordersDto);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			// 訂單主檔資訊
			var ordersDto = await (from p in _context.Orders
				where p.Orders_ID == id
				join s in _context.Customers on p.Customer_ID equals s.Customer_ID into customerGroup
				from s in customerGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new OrdersDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Orders_ID = p.Orders_ID,
				 Customer_ID = p.Customer_ID,
				 Customer_Name = s.Customer_Name,
				 Orders_Date = p.Orders_Date,
				 Orders_Status = p.Orders_Status,
				 Orders_Type = p.Orders_Type,
				 Total_Amount = p.Total_Amount,
				 Delivery_Date = p.Delivery_Date,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).FirstOrDefaultAsync();

			// 訂單明細清單
			var detailList = await (from d in _context.Orders_Detail
				where d.Orders_ID == id
				join p in _context.Products on d.Product_ID equals p.Product_ID into productGroup
				from p in productGroup.DefaultIfEmpty()
				join w in _context.Warehouse on d.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on d.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on d.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Orders_DetailDto
				{
					SequenceID = d.SequenceID,
					Orders_ID = d.Orders_ID,
					Product_ID = d.Product_ID,
					Product_Name = p.Product_Name,
					Warehouse_ID = d.Warehouse_ID,
					Warehouse_Name = w.Warehouse_Name,
					Orders_Qty = d.Orders_Qty,
					Orders_Amount = d.Orders_Amount,
					Orders_SubTotal = d.Orders_SubTotal,
					Create_Date = d.Create_Date,
					Creater = d.Creater,
					CreaterName = c != null ? c.FullName : d.Creater,
					Update_Date = d.Update_Date,
					Updater = d.Updater,
					UpdaterName = u != null ? u.FullName : d.Updater
				}).ToListAsync();

			var viewModel = new OrdersViewModel
			{
				// 訂單主檔資訊
				OrdersInfo = ordersDto,
				// 訂單明細列表
				//DetailList = _context.Purchase_Detail
				//	.Where(d => d.Purchase_ID == id).ToList()
				DetailList = detailList
			};

            return View(viewModel);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
			ViewBag.CustomerList = new SelectList(
				_context.Customers.Where(c => c.Customer_Status == "1"),
				"Customer_ID", "Customer_Name");

			// 採購單狀態
			var orders = new Orders
			{
				Orders_Status = "0" // 預設為「初始」
			};

            return View(orders);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Customer_ID,Orders_Date,Orders_Status,Orders_Type,Delivery_Date")] Orders orders)
        {
			// 在這裡由伺服器端設定建立者與建立時間
			orders.Create_Date = DateTime.Now;
			orders.Creater = UserID ?? "System";
			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			if (ModelState.ContainsKey("Orders_ID"))
				ModelState.Remove("Orders_ID");
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
			string prefix = "O" + DateTime.Now.ToString("yyyyMM");
			var maxId = _context.Orders
				.Where(p => p.Orders_ID.StartsWith(prefix))
				.OrderByDescending(p => p.Orders_ID)
				.Select(p => p.Orders_ID)
				.FirstOrDefault();
			int newSeq = 1;
			if (!string.IsNullOrEmpty(maxId) && maxId.Length >= 12)
			{
				if (int.TryParse(maxId.Substring(7, 5), out int lastSeq))
				{
					newSeq = lastSeq + 1;
				}
			}
			orders.Orders_ID = prefix + newSeq.ToString("D5");

            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
				return RedirectToAction("Edit", new { id = orders.Orders_ID });
            }
            return View(orders);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			// 客戶下拉選單
			ViewBag.CustomerList = new SelectList(
				_context.Customers.Where(c => c.Customer_Status == "1"),
				"Customer_ID", "Customer_Name");

			// 產品下拉選單
			ViewBag.ProductList = new SelectList(
				_context.Products.Where(p => p.Product_Status == "1"),
				"Product_ID", "Product_Name");

			// 倉庫下拉選單
			ViewBag.WarehouseList = new SelectList(_context.Warehouse, "Warehouse_ID", "Warehouse_Name");

			// 取得採購主檔（原始資料）
            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }

			// 訂單明細清單
			var detailList = await (from d in _context.Orders_Detail
				where d.Orders_ID == id
				join p in _context.Products on d.Product_ID equals p.Product_ID into productGroup
				from p in productGroup.DefaultIfEmpty()
				join w in _context.Warehouse on d.Warehouse_ID equals w.Warehouse_ID into warehouseGroup
				from w in warehouseGroup.DefaultIfEmpty()
				join c in _context.Sys_Accounts on d.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on d.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Orders_DetailDto
				{
					SequenceID = d.SequenceID,
					Orders_ID = d.Orders_ID,
					Product_ID = d.Product_ID,
					Product_Name = p.Product_Name,
					Warehouse_ID = d.Warehouse_ID,
					Warehouse_Name = w.Warehouse_Name,
					Orders_Qty = d.Orders_Qty,
					Orders_Amount = d.Orders_Amount,
					Orders_SubTotal = d.Orders_SubTotal,
					Create_Date = d.Create_Date,
					Creater = d.Creater,
					CreaterName = c != null ? c.FullName : d.Creater,
					Update_Date = d.Update_Date,
					Updater = d.Updater,
					UpdaterName = u != null ? u.FullName : d.Updater
				}).ToListAsync();

			var viewModel = new OrdersViewModel
			{
				// 訂單主檔表單
				OrdersForm = orders,
				// 採購單明細表單
				DetailForm = new Orders_Detail { Orders_ID = id },
				// 訂單明細列表
				DetailList = detailList
			};

            return View(viewModel);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Create_Date,Creater,Update_Date,Updater,Orders_ID,Customer_ID,Orders_Date,Orders_Status,Orders_Type,Total_Amount,Delivery_Date")] Orders orders)
        {
            if (id != orders.Orders_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.Orders_ID))
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
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.Orders_ID == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(string id)
        {
            return _context.Orders.Any(e => e.Orders_ID == id);
        }

		// 出貨
		[HttpPost]
		public async Task<IActionResult> DispatchOrder(OrdersViewModel vm)
		{
			var order = await _context.Orders.FindAsync(vm.OrdersForm.Orders_ID);
			if (order == null) return NotFound();

			var details = _context.Orders_Detail.Where(d => d.Orders_ID == order.Orders_ID).ToList();
			string connStr = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (SqlConnection conn = new SqlConnection(connStr))
				{
					await conn.OpenAsync();
					using (SqlTransaction tx = conn.BeginTransaction())
					{
						foreach (var item in details)
						{
							int qty = item.Orders_Qty ?? 0;
							string productId = item.Product_ID!;
							string warehouseId = item.Warehouse_ID ?? "WH01";

							// 查庫存
							string sqlCheck = @"
								SELECT Quantity FROM Inventory
								WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID";

							using SqlCommand checkCmd = new SqlCommand(sqlCheck, conn, tx);
							checkCmd.Parameters.AddWithValue("@ProductID", productId);
							checkCmd.Parameters.AddWithValue("@WarehouseID", warehouseId);

							object? result = await checkCmd.ExecuteScalarAsync();
							int currentQty = result != null ? Convert.ToInt32(result) : -1;

							if (currentQty < qty)
							{
								tx.Rollback();
								TempData["Error"] = $"? 產品 {productId} 庫存不足（現有：{currentQty}）";
								return RedirectToAction("Edit", new { order.Orders_ID });
							}

							// [5]已出貨時扣庫存
							if (vm.OrdersForm.Orders_Status == "5") {
								// 更新庫存
								string sqlUpdate = @"
									UPDATE Inventory
									SET Quantity = Quantity - @Qty,
										LastStockOutDate = @Date,
										Update_Date = @Date,
										Updater = @Updater
									WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID";

								using SqlCommand updateCmd = new SqlCommand(sqlUpdate, conn, tx);
								updateCmd.Parameters.AddWithValue("@Qty", qty);
								updateCmd.Parameters.AddWithValue("@Date", DateTime.Now);
								updateCmd.Parameters.AddWithValue("@Updater", UserID ?? "System");
								updateCmd.Parameters.AddWithValue("@ProductID", productId);
								updateCmd.Parameters.AddWithValue("@WarehouseID", warehouseId);

								await updateCmd.ExecuteNonQueryAsync();
							}
						}

						// 更新訂單主檔
						string sqlOrderUpdate = @"
							UPDATE Orders
							SET Orders_Status = @Status,
								Delivery_Date = @Date,
								Update_Date = @Date,
								Updater = @Updater
							WHERE Orders_ID = @OrdersID";

						using SqlCommand orderCmd = new SqlCommand(sqlOrderUpdate, conn, tx);
						orderCmd.Parameters.AddWithValue("@Status", vm.OrdersForm.Orders_Status);
						orderCmd.Parameters.AddWithValue("@Date", DateTime.Now);
						orderCmd.Parameters.AddWithValue("@Updater", UserID ?? "System");
						orderCmd.Parameters.AddWithValue("@OrdersID", order.Orders_ID);

						await orderCmd.ExecuteNonQueryAsync();

						// 提交交易
						tx.Commit();
					}

					TempData["Success"] = $"訂單 {order.Orders_ID} 出貨成功且交易已提交";
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"NG@@出貨失敗：{ex.Message}";
				return RedirectToAction("Edit", new { order.Orders_ID });
			}
			return RedirectToAction(nameof(Index));
		}

		// 新增明細
		[HttpPost]
		public async Task<IActionResult> AddDetails([Bind(Prefix = "DetailForm")] Orders_Detail detail)
		{
			var product = await _context.Products.FindAsync(detail.Product_ID);
			var inventory = _context.Inventory
				.FirstOrDefault(i => i.Product_ID == detail.Product_ID && i.Warehouse_ID == detail.Warehouse_ID);

			if (inventory == null || inventory.Quantity < (detail.Orders_Qty ?? 0))
			{
				TempData["Error"] = "庫存不足，無法新增明細";
				return RedirectToAction("Edit", new { id = detail.Orders_ID });
			}

			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			detail.Orders_Amount = product?.Price ?? 0;
			detail.Orders_SubTotal = (detail.Orders_Qty ?? 0) * (product?.Price ?? 0);
			detail.Create_Date = DateTime.Now;
			detail.Creater = UserID ?? "System";

			_context.Orders_Detail.Add(detail);
			await _context.SaveChangesAsync();

			TempData["Success"] = "明細新增成功";
			return RedirectToAction("Edit", new { id = detail.Orders_ID });
		}

		// 刪除明細
		public async Task<IActionResult> DeleteDetails(int id, string ordersId)
		{
			var detail = await _context.Orders_Detail.FindAsync(id);
			if (detail == null) return NotFound();

			_context.Orders_Detail.Remove(detail);
			await _context.SaveChangesAsync();

			TempData["Success"] = "?? 已刪除明細";
			return RedirectToAction("Edit", new { id = ordersId });
		}

		// 轉銷貨
		[HttpPost]
		public async Task<IActionResult> ConvertToSales(OrdersViewModel vm)
		{
			var orderId = vm.OrdersInfo?.Orders_ID;
			if (string.IsNullOrEmpty(orderId))
			{
				TempData["Error"] = "NG@@訂單編號未提供，無法建立銷貨單";
				return RedirectToAction("Details", new { id = orderId });
			}

			var order = await _context.Orders.FindAsync(orderId);
			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			if (order == null)
			{
				TempData["Error"] = $"NG@@查無訂單 {orderId}";
				return RedirectToAction("Details", new { id = orderId });
			}

			var sales = new Sales
			{
				Sales_ID = "S" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
				Orders_ID = order.Orders_ID,
				Sales_Date = DateTime.Now,
				Shipping_Date = DateTime.Now,
				Sales_Status = "0",
				Create_Date = DateTime.Now,
				Creater = order.Creater,
				Total_Amount = order.Total_Amount
			};
			_context.Sales.Add(sales);

			var details = await _context.Orders_Detail
				.Where(d => d.Orders_ID == orderId).ToListAsync();

			foreach (var item in details)
			{
				var sd = new Sales_Detail
				{
					Sales_ID = sales.Sales_ID,
					Product_ID = item.Product_ID,
					Sales_Qty = item.Orders_Qty,
					Sales_Amount = item.Orders_Amount,
					Sales_SubTotal = item.Orders_SubTotal,
					Create_Date = DateTime.Now,
					Creater = order.Creater
				};
				_context.Sales_Detail.Add(sd);
			}

			await _context.SaveChangesAsync();
			TempData["SalesSuccess"] = $"OK@@已從訂單 {orderId} 建立銷貨單 {sales.Sales_ID}";
			return RedirectToAction("Edit", "Sales", new { id = sales.Sales_ID });
		}
    }
}
