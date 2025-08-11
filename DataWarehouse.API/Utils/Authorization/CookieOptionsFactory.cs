using Microsoft.AspNetCore.Http;

namespace DataWarehouse.API.Utils.Authorization
{
    public static class CookieOptionsFactory
    {
        public static CookieOptions CreateRefreshCookie(
            IWebHostEnvironment env,
            TimeSpan ttl,
            bool expireNow = false)
        {
            var isProd = env.IsProduction();

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isProd,                      
                SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
                Path = "/",
                Expires = expireNow
                    ? DateTimeOffset.UtcNow.AddDays(-1)
                    : DateTimeOffset.UtcNow.Add(ttl),
                IsEssential = true
            };
        }
    }
}