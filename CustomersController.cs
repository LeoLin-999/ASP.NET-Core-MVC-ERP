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
	// �Ȥᱱ�
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
				Customer_Status = "0" // �w�]���u��l�v
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
			// �b�o�̥Ѧ��A���ݳ]�w�إߪ̻P�إ߮ɶ�
			customers.Create_Date = DateTime.Now;
			customers.Creater = UserID ?? "System";
			// ���o����쪺 ModelState ���~
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");
			// �����T��
			if (!ModelState.IsValid)
			{
				foreach (var modelState in ModelState)
				{
					foreach (var error in modelState.Value.Errors)
					{
						Console.WriteLine($"���: {modelState.Key}, ���~: {error.ErrorMessage}");
					}
				}
			}
            if (ModelState.IsValid)
            {
				// �ˬd�Ȥ�N���O�_�w�s�b
				bool isExists = await _context.Customers
					.AnyAsync(a => a.Customer_ID == customers.Customer_ID);
				if (isExists)
				{
					ModelState.AddModelError("Customer_ID", "�Ȥ�N���w�s�b�A�ШϥΨ�L�N���I");
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

			// ���o����쪺 ModelState ���~
			if (ModelState.ContainsKey("Create_Date"))
				ModelState.Remove("Create_Date");
			if (ModelState.ContainsKey("Creater"))
				ModelState.Remove("Creater");

			// �����T��
			if (!ModelState.IsValid)
			{
				foreach (var modelState in ModelState)
				{
					foreach (var error in modelState.Value.Errors)
					{
						Console.WriteLine($"���: {modelState.Key}, ���~: {error.ErrorMessage}");
					}
				}
			}

			// �b�o�̥Ѧ��A���ݳ]�w�ק�̻P�ק�ɶ�
			customers.Update_Date = DateTime.Now;
			customers.Updater = UserID ?? "System";

			// ���o��Ʈw�s�u�r��
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
				ModelState.AddModelError("", "��s��Ʈɵo�Ϳ��~�G" + ex.Message);
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
