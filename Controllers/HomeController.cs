using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcERPTest01.Models;

namespace MvcERPTest01.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
	private readonly ErpDbContext _context;

    public HomeController(ILogger<HomeController> logger, ErpDbContext context)
    {
        _logger = logger;
		_context = context;
    }

	[HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

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
			//return RedirectToAction("Dashboard", "Home");
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

	public IActionResult Dashboard()
	{
		ViewData["Title"] = "Dashboard";
		return View();
	}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
