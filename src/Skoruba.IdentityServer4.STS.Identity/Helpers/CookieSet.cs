using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class CookieSet
    {
        private readonly RequestDelegate _next;
        private readonly SessionOptions _options;
        private HttpContext _context;
        public CookieSet(RequestDelegate next, IOptions<SessionOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            _context = context;
            if (context.Request.Path == "/connect/token")
                context.Response.OnStarting(OnStartingCallBack);
            await _next.Invoke(context);
        }

        private Task OnStartingCallBack()
        {
            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                // Expires = DateTimeOffset.UtcNow.AddHours(1),
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            var fingerPrint = _context.Items.ContainsKey("CookieFingerprint") ? (string)_context.Items["CookieFingerprint"] : null;
            if (fingerPrint != null)
                _context.Response.Cookies.Append("__Secure-Fgp", fingerPrint, cookieOptions);
            return Task.FromResult(0);
        }
    }
}
