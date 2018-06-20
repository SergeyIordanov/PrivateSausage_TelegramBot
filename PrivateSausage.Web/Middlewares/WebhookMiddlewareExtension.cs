using System;
using Microsoft.AspNetCore.Builder;

namespace PrivateSausage.Web.Middlewares
{
    public static class WebhookMiddlewareExtension
    {
        public static IApplicationBuilder UseWebhook(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<WebhookMiddleware>();
        }
    }
}
