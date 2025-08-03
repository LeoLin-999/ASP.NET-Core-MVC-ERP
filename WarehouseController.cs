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
	// �ܮw���
    public class WarehouseController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public WarehouseController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Warehouse
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Warehouse.ToListAsync());
			var warehouseDto = await (from s in _context.Warehouse
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new WarehouseDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Warehouse_ID = s.Warehouse_ID,
				 Warehouse_Name = s.Warehouse_Name,
				 Warehouse_Status = s.Warehouse_Status,
				 Total_Quantity = s.Total_Quantity,
				 Max_Quantity = s.Max_Quantity,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).ToListAsync();
            return View(warehouseDto);
        }

        // GET: Warehouse/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //return View(await _context.Warehouse.ToListAsync());
			var warehouseDto = await (from s in _context.Warehouse
				where s.Warehouse_ID == id
				join c in _context.Sys_Accounts on s.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on s.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new WarehouseDto
				{
				 Create_Date = s.Create_Date,
				 Creater = s.Creater,
				 Update_Date = s.Update_Date,
				 Updater = s.Updater,
				 Warehouse_ID = s.Warehouse_ID,
				 Warehouse_Name = s.Warehouse_Name,
				 Warehouse_Status = s.Warehouse_Status,
				 Total_Quantity = s.Total_Quantity,
				 Max_Quantity = s.Max_Quantity,
				 CreaterName = c != null ? c.FullName : s.Creater,
				 UpdaterName = u != null ? u.FullName : s.Updater
				}).FirstOrDefaultAsync();

            return View(warehouseDto);
        }

        // GET: Warehouse/Create
        public IActionResult Create()
        {
			var warehouse = new Warehouse
			{
				Warehouse_Status = "0" // �w�]���u��l�v
			};
            return View(warehouse);
        }

        // POST: Warehouse/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Warehouse_ID,Warehouse_Name,Warehouse_Status,Total_Quantity,Max_Quantity")] Warehouse warehouse)
        {
			// �b�o�̥Ѧ��A���ݳ]�w�إߪ̻P�إ߮ɶ�
			warehouse.Create_Date = DateTime.Now;
			warehouse.Creater = UserID ?? "System";
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
				// �ˬd�ܮw�N���O�_�w�s�b
				bool isExists = await _context.Warehouse
					.AnyAsync(a => a.Warehouse_ID == warehouse.Warehouse_ID);
				if (isExists)
				{
					ModelState.AddModelError("Warehouse_ID", "�ܮw�N���w�s�b�A�ШϥΨ�L�N���I");
					return View(warehouse);
				}
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: Warehouse/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouse.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouse/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Warehouse_ID,Warehouse_Name,Warehouse_Status,Total_Quantity,Max_Quantity")] Warehouse warehouse)
        {
            if (id != warehouse.Warehouse_ID)
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
			warehouse.Update_Date = DateTime.Now;
			warehouse.Updater = UserID ?? "System";

			// ���o��Ʈw�s�u�r��
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand(@"
					UPDATE Warehouse
					SET Warehouse_Name = @warehouse_Name
						,Warehouse_Status = @warehouse_Status
						,Total_Quantity = @total_Quantity
						,Max_Quantity = @max_Quantity
						,Update_Date = @updateDate
						,Updater = @updater
					WHERE Warehouse_ID = @warehouse_ID
				", connection))
				{
					command.Parameters.AddWithValue("@warehouse_Name", warehouse.Warehouse_Name ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@warehouse_Status", warehouse.Warehouse_Status ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@total_Quantity", warehouse.Total_Quantity ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@max_Quantity", warehouse.Max_Quantity ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@updateDate", warehouse.Update_Date);
					command.Parameters.AddWithValue("@updater", warehouse.Updater ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@warehouse_ID", warehouse.Warehouse_ID);

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
				return View(warehouse);
			}
        }

        // GET: Warehouse/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouse
                .FirstOrDefaultAsync(m => m.Warehouse_ID == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var warehouse = await _context.Warehouse.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouse.Remove(warehouse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseExists(string id)
        {
            return _context.Warehouse.Any(e => e.Warehouse_ID == id);
        }
    }
}
