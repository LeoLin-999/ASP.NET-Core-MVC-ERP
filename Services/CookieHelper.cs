using Microsoft.AspNetCore.Http;	// 引入 HttpRequest 和 HttpResponse
using System.Collections.Generic;
using System.Text.Json;

public static class CookieHelper
{
    public static void SetCookieValues(HttpResponse response, string cookieName, Dictionary<string, string> values, int expireHours = 1)
    {
        string encoded = string.Join("&", values.Select(kv => $"{kv.Key}={kv.Value}"));
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddHours(expireHours),
            HttpOnly = true,
            Secure = true
        };
        response.Cookies.Append(cookieName, encoded, options);
    }

    public static Dictionary<string, string> GetCookieValues(HttpRequest request, string cookieName)
    {
        var result = new Dictionary<string, string>();
        string? raw = request.Cookies[cookieName];

        if (!string.IsNullOrEmpty(raw))
        {
            var pairs = raw.Split('&');
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=');
                if (kv.Length == 2)
                {
                    result[kv[0]] = kv[1];
                }
            }
        }

        return result;
    }

    public static Dictionary<string, string> ParseCookie(string? rawValue)
    {
        var result = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(rawValue))
        {
            var pairs = rawValue.Split('&');
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=');
                if (kv.Length == 2)
                {
                    result[kv[0]] = kv[1];
                }
            }
        }

        return result;
    }

    public static void RemoveCookie(HttpResponse response, string cookieName)
    {
        response.Cookies.Delete(cookieName);
    }
}