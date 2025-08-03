using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcERPTest01.Models;

namespace MvcERPTest01.Controllers;

public class ErpController : BaseController
{
	private readonly ErpDbContext _context;

    public ErpController(ErpDbContext context)
    {
		_context = context;
    }

    public IActionResult Index()
    {
		ViewData["Title"] = "Enterprise Resource Planning";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
