using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using PrivateSausage.Web.Configuration;

namespace PrivateSausage.Web.Filters
{
    public class BotTokenValidationFilterAttribute : ActionFilterAttribute
    {
        private const string TokenRouteDataKey = "token";

        private readonly BotOptions _options;

        public BotTokenValidationFilterAttribute(IOptions<BotOptions> options)
        {
            _options = options.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var tokenProvided = context.RouteData.Values.TryGetValue(TokenRouteDataKey, out object token);

            if (!tokenProvided || !ValidateToken(token))
            {
                context.Result = new BadRequestObjectResult("Bot token is invalid.");
            }

            base.OnActionExecuting(context);
        }

        private bool ValidateToken(object token)
        {
            return string.Equals(token.ToString(), _options.Token, StringComparison.InvariantCulture);
        }
    }
}
