using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.CustomMiddleWares;

public class PaginationMiddleware
{
    private readonly RequestDelegate _next;

    public PaginationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (int.TryParse(context.Request.Query["pageNumber"], out var pageNumber))
        {
            if (pageNumber < 1)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            const int totalItems = 1000; // replace with actual total number of items
            const int pageSize = 12;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (pageNumber > totalPages)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        await _next(context);
    }
}