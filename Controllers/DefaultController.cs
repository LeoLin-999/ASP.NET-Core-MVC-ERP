using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcERPTest01.Models;

namespace MvcERPTest01.Controllers;

public class DefaultController : Controller
{
	private readonly ErpDbContext _context;

    public DefaultController(ErpDbContext context)
    {
		_context = context;
    }

    public IActionResult Index()
    {
        var cookieValue = Request.Cookies["UserInfo"];
        
        if (string.IsNullOrEmpty(cookieValue))
        {
            // Cookie 不存在 → 導向登入畫面
            return RedirectToAction("Login", "Default");
        }

        // Cookie 存在 → 導向 ERP 主頁
        return RedirectToAction("Index", "Erp");
    }

	[HttpGet]
	public IActionResult Login()
	{
		return View();	// 對應 Views/Default/Login.cshtml
	}

	// 登入驗證
	[HttpPost]
	public IActionResult Login(LoginViewModel model)
	{
		//if (!ModelState.IsValid) return View(model);
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

		var user = _context.Sys_Accounts
			.FirstOrDefault(x => x.AccountID == model.AccountID && x.Password == model.Password);

		if (user != null)
		{
			// 登入成功，設定 Cookie
			CookieOptions options = new CookieOptions
			{
				Expires = DateTime.Now.AddHours(1),
				HttpOnly = true,
				Secure = true
			};
			//Response.Cookies.Append("ERPLogin", user.AccountID, options);
			var values = new Dictionary<string, string>
			{
				{ "UserID", user.AccountID },
				{ "FullName", user.FullName ?? "" }
			};
			CookieHelper.SetCookieValues(Response, "UserInfo", values);

			// 驗證成功，導向另一個 layout 的頁面，例如 Dashboard
			return RedirectToAction("Index", "Erp");
		}

		ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
		return View(model);
	}

    public IActionResult Logout()
    {
        CookieHelper.RemoveCookie(Response, "UserInfo");
        return RedirectToAction("Login");
    }

	// GET: AccountRegister
	public IActionResult AccountRegister()
	{
		return View();
	}

	// POST: AccountRegister
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AccountRegister([Bind("AccountID,Password,FullName,EMail")] Sys_Accounts sys_Accounts)
	{
		// 在這裡由伺服器端設定建立者與建立時間
		sys_Accounts.Create_Date = DateTime.Now;
		sys_Accounts.Creater = User.Identity?.Name ?? "System";
		sys_Accounts.AccountStatus = "0";
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
			return RedirectToAction(nameof(Login));
		}
		return View(sys_Accounts);
	}
}