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
	// 客戶控制器
    public class CustomersController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public CustomersController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Customers.ToListAsync());
			var customersDto = await (from s in _context.Customers
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new CustomersDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Customer_ID = s.Customer_ID,
				 Customer_Name = s.Customer_Name,
				 Customer_Status = s.Customer_Status,
				 Customer_EMail = s.Customer_EMail,
				 Customer_Address = s.Customer_Address,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).ToListAsync();
            return View(customersDto);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var customersDto = await (from s in _context.Customers
				where s.Customer_ID == id
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new CustomersDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Customer_ID = s.Customer_ID,
				 Customer_Name = s.Customer_Name,
				 Customer_Status = s.Customer_Status,
				 Customer_EMail = s.Customer_EMail,
				 Customer_Address = s.Customer_Address,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).FirstOrDefaultAsync();
            return View(customersDto);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
			var customers = new Customers
			{
				Customer_Status = "0" // 預設為「初始」
			};
            return View(customers);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Customer_ID,Customer_Name,Customer_Status,Customer_EMail,Customer_Address")] Customers customers)
        {
			// 在這裡由伺服器端設定建立者與建立時間
			customers.Create_Date = DateTime.Now;
			customers.Creater = UserID ?? "System";
			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
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
            if (ModelState.IsValid)
            {
				// 檢查客戶代號是否已存在
				bool isExists = await _context.Customers
					.AnyAsync(a => a.Customer_ID == customers.Customer_ID);
				if (isExists)
				{
					ModelState.AddModelError("Customer_ID", "客戶代號已存在，請使用其他代號！");
					return View(customers);
				}
                _context.Add(customers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Customer_ID,Customer_Name,Customer_Status,Customer_EMail,Customer_Address")] Customers customers)
        {
            if (id != customers.Customer_ID)
            {
                return NotFound();
            }

			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");

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

			// 在這裡由伺服器端設定修改者與修改時間
			customers.Update_Date = DateTime.Now;
			customers.Updater = UserID ?? "System";

			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand(@"
					UPDATE Customers
					SET Customer_Name = @customer_Name
						,Customer_Status = @customer_Status
						,Customer_EMail = @customer_EMail
						,Customer_Address = @customer_Address
						,Update_Date = @updateDate
						,Updater = @updater
					WHERE Customer_ID = @supplier_ID
				", connection))
				{
					command.Parameters.AddWithValue("@customer_Name", customers.Customer_Name ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@customer_Status", customers.Customer_Status ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@customer_EMail", customers.Customer_EMail ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@customer_Address", customers.Customer_Address ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@updateDate", customers.Update_Date);
					command.Parameters.AddWithValue("@updater", customers.Updater ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@supplier_ID", customers.Customer_ID);

					await connection.OpenAsync();
					int affectedRows = await command.ExecuteNonQueryAsync();

					if (affectedRows == 0)
						return NotFound();
				}

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "更新資料時發生錯誤：" + ex.Message);
				return View(customers);
			}
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .FirstOrDefaultAsync(m => m.Customer_ID == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers != null)
            {
                _context.Customers.Remove(customers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomersExists(string id)
        {
            return _context.Customers.Any(e => e.Customer_ID == id);
        }
    }
}
