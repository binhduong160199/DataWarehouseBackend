using System.Security.Claims;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            // Example: Only allow Admins to access /api/admin/*
            if (context.Request.Path.StartsWithSegments("/api/admin") && role != "Admin")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied");
                return;
            }
        }

        await _next(context);
    }
}