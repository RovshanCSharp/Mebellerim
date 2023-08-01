using System;
using System.Threading.Tasks;
using Mebeller.Data.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.CustomMiddleWares;

public class VisitorCounterMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    public VisitorCounterMiddleware(RequestDelegate requestDelegate) => _requestDelegate = requestDelegate;

    public async Task Invoke(HttpContext context, IVisitorService visitorService)
    {
        if (context.Request.Cookies["VisitorId"] == null)
        {
            await visitorService.AddOrUpdateVisitorAsync(context.Connection.RemoteIpAddress?.ToString());
            context.Response.Cookies.Append("VisitorId", Guid.NewGuid().ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = false
            });
        }

        await _requestDelegate(context);
    }
}