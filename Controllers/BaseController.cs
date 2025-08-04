using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace MvcERPTest01.Controllers;

public class BaseController : Controller
{
    protected Dictionary<string, string>? LoginInfo;
	public string UserID { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        string? cookie = context.HttpContext.Request.Cookies["UserInfo"];
        LoginInfo = CookieHelper.ParseCookie(cookie);

        if (LoginInfo == null || !LoginInfo.ContainsKey("UserID"))
        {
            context.Result = new RedirectToActionResult("Login", "Default", null);
        }
        else
        {
			UserID = LoginInfo["UserID"];
            ViewData["UserID"] = LoginInfo["UserID"];
            ViewData["FullName"] = LoginInfo["FullName"];
        }
    }
}
