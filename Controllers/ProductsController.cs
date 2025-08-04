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
	// ���~���
    public class ProductsController : BaseController
    {
        private readonly ErpDbContext _context;
		private readonly IConfiguration _configuration;

        public ProductsController(ErpDbContext context, IConfiguration configuration)
        {
            _context = context;
			_configuration = configuration;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Products.ToListAsync());
			var productsDto = await (from p in _context.Products
				join c in _context.Sys_Accounts on p.Creater equals c.AccountID into createrGroup
				from c in createrGroup.DefaultIfEmpty()
				join u in _context.Sys_Accounts on p.Updater equals u.AccountID into updaterGroup
				from u in updaterGroup.DefaultIfEmpty()
				select new ProductsDto
				{
				 Create_Date = p.Create_Date,
				 Creater = p.Creater,
				 Update_Date = p.Update_Date,
				 Updater = p.Updater,
				 Product_ID = p.Product_ID,
				 Product_Name = p.Product_Name,
				 Product_Status = p.Product_Status,
				 Price = p.Price,
				 CreaterName = c != null ? c.FullName : p.Creater,
				 UpdaterName = u != null ? u.FullName : p.Updater
				}).ToListAsync();
            return View(productsDto);
        }

        // GET: Products/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productsDto = await (from p in _context.Products
									where p.Product_ID == id
									join c in _context.Sys_Accounts on p.Creater equals c.AccountID into createrGroup
									from c in createrGroup.DefaultIfEmpty()
									join u in _context.Sys_Accounts on p.Updater equals u.AccountID into updaterGroup
									from u in updaterGroup.DefaultIfEmpty()
									select new ProductsDto
									{
										Create_Date = p.Create_Date,
										Creater = p.Creater,
										Update_Date = p.Update_Date,
										Updater = p.Updater,
										Product_ID = p.Product_ID,
										Product_Name = p.Product_Name,
										Product_Status = p.Product_Status,
										Price = p.Price,
										CreaterName = c != null ? c.FullName : p.Creater,
										UpdaterName = u != null ? u.FullName : p.Updater
									}).FirstOrDefaultAsync();

			if (productsDto == null)
			{
				return NotFound();
			}

			return View(productsDto);
		}

        // GET: Products/Create
        public IActionResult Create()
        {
			var products = new Products
			{
				Product_Status = "0" // �w�]���u��l�v
			};
            return View(products);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Product_ID,Product_Name,Product_Status,Price")] Products products)
        {
			// �b�o�̥Ѧ��A���ݳ]�w�إߪ̻P�إ߮ɶ�
			products.Create_Date = DateTime.Now;
			products.Creater = UserID ?? "System";
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
				// �ˬd���~�N���O�_�w�s�b
				bool productsExists = await _context.Products
					.AnyAsync(a => a.Product_ID == products.Product_ID);
				if (productsExists)
				{
					ModelState.AddModelError("Product_ID", "���~�N���w�s�b�A�ШϥΨ�L�N���I");
					return View(products);
				}
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Product_ID,Product_Name,Product_Status,Price")] Products products)
        {
            if (id != products.Product_ID)
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
			products.Update_Date = DateTime.Now;
			products.Updater = UserID ?? "System";

			// ���o��Ʈw�s�u�r��
			var connectionString = _configuration.GetConnectionString("ErpDbContext");

			try
			{
				using (var connection = new SqlConnection(connectionString))
				using (var command = new SqlCommand(@"
					UPDATE Products
					SET Product_Status = @product_Status
						,Product_Name = @product_Name
						,Price = @price
						,Update_Date = @updateDate
						,Updater = @updater
					WHERE Product_ID = @product_ID
				", connection))
				{
					command.Parameters.AddWithValue("@product_Status", products.Product_Status ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@product_Name", products.Product_Name ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@price", products.Price ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@updateDate", products.Update_Date);
					command.Parameters.AddWithValue("@updater", products.Updater ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@product_ID", products.Product_ID);

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
				return View(products);
			}
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Product_ID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(string id)
        {
            return _context.Products.Any(e => e.Product_ID == id);
        }
    }
}
