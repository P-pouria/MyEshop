using Microsoft.AspNetCore.Builder;

namespace YourNamespace.Middleware
{
    public static class AdminAccessMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAccessMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAccessMiddleware>();
        }
    }
}
