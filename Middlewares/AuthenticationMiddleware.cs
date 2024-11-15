using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace WarehouseManagementSystem.Middlewares
{
    public class AuthenticationMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            filterContext.HttpContext.TraceIdentifier = GuidId.ToString();
            IDictionary<string, object> Parameters = filterContext.ActionArguments;

            throw new NotImplementedException();
        }
    }
}
