    using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Threading.Tasks;
using WarehouseManagementSystem.Helper;
using WarehouseManagementSystem.Infrastructure.JwtService;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string? token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[0];
           
            if(token == null)
            {
                // check if request from enable unauthorized route
                if (IEneabledUnauthorizedRoute(httpContext))
                {
                    return _next(httpContext);
                }

                BaseResponse<object> response = new BaseResponse<object>("unauthorized", false, 401);
                
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.ContentType = "application/json";

                return httpContext.Response.WriteAsJsonAsync(response);
            }
            else
            {
                if (JwtHelper.ValidateJwtToken(token))
                {
                    if(JwtHelper.CheckUserPermissions(token, httpContext))
                    {

                        return _next(httpContext);
                    }
                    else
                    {
                        BaseResponse<object> response = new BaseResponse<object>("Forbidden", false, 403);

                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        httpContext.Response.ContentType = "application/json";

                        return httpContext.Response.WriteAsJsonAsync(response);
                    }
                }
                else
                {
                    BaseResponse<object> response = new BaseResponse<object>("unauthorized", false, 401);

                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    httpContext.Response.ContentType = "application/json";

                    return httpContext.Response.WriteAsJsonAsync(response);
                }
            }
        }

        // this method used for ckeck incoming Requst is from Eneabled Unauthorized
        private bool IEneabledUnauthorizedRoute(HttpContext httpContext)
        {
            List<string> enableRoutes = new List<string>
            {
                "/api/Authentication/LogIn",
                "/api/Authentication/ApplySeeder",
                "/api/Authentication/MigrateDatabase"
            };

            bool iEneabledUnauthorizedRoute = false;

            if (httpContext.Request.Path.Value is not null)
            {
                iEneabledUnauthorizedRoute = enableRoutes.Contains(httpContext.Request.Path.Value);
            }

            return iEneabledUnauthorizedRoute;
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
