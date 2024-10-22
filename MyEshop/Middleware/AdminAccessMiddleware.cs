using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YourNamespace.Middleware
{
    public class AdminAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("/Account/Login");
                    return; 
                }

                var isAdminClaim = context.User.FindFirstValue("IsAdmin");
                if (string.IsNullOrEmpty(isAdminClaim) || !bool.Parse(isAdminClaim))
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
