using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pro.backend.Helpers
{
    public class UserDestroyerMiddleware
    {
        private readonly RequestDelegate _next;

        public UserDestroyerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {

            bool canParse = int.TryParse(httpContext.User.Identity.Name, out var Id);
            if (!string.IsNullOrEmpty(httpContext.User.Identity.Name)&&!canParse)
            {
                var user = await userManager.FindByIdAsync(httpContext.User.Identity.Name);

                if (user!=null && await userManager.IsLockedOutAsync(user))
                {
                   httpContext.Response.StatusCode = 701;
                   return;
                }
            }
            await _next(httpContext);
        }
    }

    public static class UserDestroyerMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserDestroyer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserDestroyerMiddleware>();
        }
    }
}