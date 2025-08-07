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
	// 帳號控制器
    public class Sys_AccountsController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;
		// AES加解密服務
		private readonly AesHelper _aesHelper;

		public Sys_AccountsController(ErpDbContext context, IConfiguration configuration, AesHelper aesHelper)
		{
			_context = context;
			_configuration = configuration;
			_aesHelper = aesHelper;
		}

        // GET: Sys_Accounts
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Sys_Accounts.ToListAsync());
			var sys_AccountsDto = await (from s in _context.Sys_Accounts
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Sys_AccountsDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 AccountID = s.AccountID,
				 AccountStatus = s.AccountStatus,
				 FullName = s.FullName,
				 EMail = s.EMail,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).ToListAsync();
            return View(sys_AccountsDto);
        }

        // GET: Sys_Accounts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var sys_AccountsDto = await (from s in _context.Sys_Accounts
				where s.AccountID == id
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new Sys_AccountsDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 AccountID = s.AccountID,
				 AccountStatus = s.AccountStatus,
				 FullName = s.FullName,
				 EMail = s.EMail,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater,
				 Password = _aesHelper.Decrypt(s.Password)
				}).FirstOrDefaultAsync();

            return View(sys_AccountsDto);
        }

        // GET: Sys_Accounts/Create
        public IActionResult Create()
        {
			var sys_Accounts = new Sys_Accounts
			{
				AccountStatus = "0" // 預設為「初始」
			};
            return View(sys_Accounts);
        }

        // POST: Sys_Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountID,AccountStatus,Password,FullName,EMail")] Sys_Accounts sys_Accounts)
        {
			// 在這裡由伺服器端設定建立者與建立時間
			sys_Accounts.Create_Date = DateTime.Now;
			sys_Accounts.Creater = UserID ?? "System";
			// 除這些欄位的 ModelState 錯誤
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			// 密碼加密
			if (!string.IsNullOrEmpty(sys_Accounts.Password))
				sys_Accounts.Password = _aesHelper.Encrypt(sys_Accounts.Password);
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
				// 檢查帳號是否已存在
				bool accountExists = await _context.Sys_Accounts
					.AnyAsync(a => a.AccountID == sys_Accounts.AccountID);

				if (accountExists)
				{
					ModelState.AddModelError("AccountID", "帳號已存在，請使用其他帳號！");
					return View(sys_Accounts);
				}

                _context.Add(sys_Accounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sys_Accounts);
        }

        // GET: Sys_Accounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sys_Accounts = await _context.Sys_Accounts.FindAsync(id);
            if (sys_Accounts == null)
            {
                return NotFound();
            }
            return View(sys_Accounts);
        }

        // POST: Sys_Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("AccountID,AccountStatus,Password,FullName,EMail")] Sys_Accounts sys_Accounts)
		{
			if (id != sys_Accounts.AccountID)
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

			sys_Accounts.Update_Date = DateTime.Now;
			if (sys_Accounts.Update_Date < new DateTime(1753, 1, 1))
			{
				sys_Accounts.Update_Date = new DateTime(1753, 1, 1);
			}
			sys_Accounts.Updater = UserID ?? "System";

			// 密碼加密
			if (!string.IsNullOrEmpty(sys_Accounts.Password))
				sys_Accounts.Password = _aesHelper.Encrypt(sys_Accounts.Password);

			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand(@"
					UPDATE Sys_Accounts
					SET AccountStatus = @accountStatus
						,Password = @password
						,FullName = @fullName
						,EMail = @email
						,Update_Date = @updateDate
						,Updater = @updater
					WHERE AccountID = @accountId
				", connection))
				{
					command.Parameters.AddWithValue("@accountStatus", sys_Accounts.AccountStatus ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@password", sys_Accounts.Password ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@fullName", sys_Accounts.FullName ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@email", sys_Accounts.EMail ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@updateDate", sys_Accounts.Update_Date);
					command.Parameters.AddWithValue("@updater", sys_Accounts.Updater ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@accountId", sys_Accounts.AccountID);

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
				return View(sys_Accounts);
			}
		}

        // GET: Sys_Accounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sys_Accounts = await _context.Sys_Accounts
                .FirstOrDefaultAsync(m => m.AccountID == id);
            if (sys_Accounts == null)
            {
                return NotFound();
            }

            return View(sys_Accounts);
        }

        // POST: Sys_Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sys_Accounts = await _context.Sys_Accounts.FindAsync(id);
            if (sys_Accounts != null)
            {
                _context.Sys_Accounts.Remove(sys_Accounts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sys_AccountsExists(string id)
        {
            return _context.Sys_Accounts.Any(e => e.AccountID == id);
        }
    }
}
