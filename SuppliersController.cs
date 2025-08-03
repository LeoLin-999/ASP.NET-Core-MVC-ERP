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
	// 供應商控制器
    public class SuppliersController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public SuppliersController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Suppliers.ToListAsync());
			var suppliersDto = await (from s in _context.Suppliers
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new SuppliersDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Supplier_ID = s.Supplier_ID,
				 Supplier_Name = s.Supplier_Name,
				 Supplier_Status = s.Supplier_Status,
				 Supplier_EMail = s.Supplier_EMail,
				 Supplier_Address = s.Supplier_Address,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).ToListAsync();
            return View(suppliersDto);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var suppliersDto = await (from s in _context.Suppliers
				where s.Supplier_ID == id
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new SuppliersDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Supplier_ID = s.Supplier_ID,
				 Supplier_Name = s.Supplier_Name,
				 Supplier_Status = s.Supplier_Status,
				 Supplier_EMail = s.Supplier_EMail,
				 Supplier_Address = s.Supplier_Address,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).FirstOrDefaultAsync();
            return View(suppliersDto);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
			var suppliers = new Suppliers
			{
				Supplier_Status = "0" // 預設為「初始」
			};
            return View(suppliers);
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Supplier_ID,Supplier_Name,Supplier_Status,Supplier_EMail,Supplier_Address")] Suppliers suppliers)
        {
			// 在這裡由伺服器端設定建立者與建立時間
			suppliers.Create_Date = DateTime.Now;
			suppliers.Creater = UserID ?? "System";
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
				// 檢查供應商代號是否已存在
				bool isExists = await _context.Suppliers
					.AnyAsync(a => a.Supplier_ID == suppliers.Supplier_ID);
				if (isExists)
				{
					ModelState.AddModelError("Supplier_ID", "倉庫代號已存在，請使用其他代號！");
					return View(suppliers);
				}
                _context.Add(suppliers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers == null)
            {
                return NotFound();
            }
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Supplier_ID,Supplier_Name,Supplier_Status,Supplier_EMail,Supplier_Address")] Suppliers suppliers)
        {
            if (id != suppliers.Supplier_ID)
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
			suppliers.Update_Date = DateTime.Now;
			suppliers.Updater = UserID ?? "System";

			// 取得資料庫連線字串
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand(@"
					UPDATE Suppliers
					SET Supplier_Name = @supplier_Name
						,Supplier_Status = @supplier_Status
						,Supplier_EMail = @supplier_EMail
						,Supplier_Address = @supplier_Address
						,Update_Date = @updateDate
						,Updater = @updater
					WHERE Supplier_ID = @supplier_ID
				", connection))
				{
					command.Parameters.AddWithValue("@supplier_Name", suppliers.Supplier_Name ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@supplier_Status", suppliers.Supplier_Status ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@supplier_EMail", suppliers.Supplier_EMail ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@supplier_Address", suppliers.Supplier_Address ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@updateDate", suppliers.Update_Date);
					command.Parameters.AddWithValue("@updater", suppliers.Updater ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@supplier_ID", suppliers.Supplier_ID);

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
				return View(suppliers);
			}
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suppliers = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Supplier_ID == id);
            if (suppliers == null)
            {
                return NotFound();
            }

            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers != null)
            {
                _context.Suppliers.Remove(suppliers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuppliersExists(string id)
        {
            return _context.Suppliers.Any(e => e.Supplier_ID == id);
        }
    }
}
