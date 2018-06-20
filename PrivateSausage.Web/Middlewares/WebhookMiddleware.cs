using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using PrivateSausage.Web.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace PrivateSausage.Web.Middlewares
{
    public class WebhookMiddleware
    {
        private const string TokenRouteVariable = "{token}";

        private readonly RequestDelegate _next;
        private readonly IActionResultExecutor<ObjectResult> _objectResultExecutor;
        private readonly ITelegramBotClient _botClient;
        private readonly BotOptions _options;

        public WebhookMiddleware(
            RequestDelegate next,
            IActionResultExecutor<ObjectResult> objectResultExecutor,
            ITelegramBotClient botClient,
            IOptions<BotOptions> options)
        {
            _next = next;
            _botClient = botClient;
            _options = options.Value;
            _objectResultExecutor = objectResultExecutor;
        }

        public async Task Invoke(HttpContext context)
        {
            var webHookInfo = await _botClient.GetWebhookInfoAsync();

            var webhookExists = !string.IsNullOrEmpty(webHookInfo.Url);

            if (webhookExists)
            {
                if (!ValidateWebhookUri(webHookInfo))
                {
                    await _botClient.DeleteWebhookAsync();

                    await CreateWebhookAsync(context);
                }
            }
            else
            {
                await CreateWebhookAsync(context);
            }

            await _next(context);
        }

        private async Task CreateWebhookAsync(HttpContext context)
        {
            try
            {
                await _botClient.SetWebhookAsync(
                    _options.Webhook.Url.Replace(TokenRouteVariable, _options.Token),
                    null,
                    _options.Webhook.MaxConnections,
                    _options.Webhook.AllowedUpdates);
            }
            catch (ApiRequestException exception) when (exception.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                await CreateResponse(context, exception, (int)HttpStatusCode.BadRequest);
            }
        }

        private bool ValidateWebhookUri(WebhookInfo webHookInfo)
        {
            return string.Equals(webHookInfo.Url, _options.Webhook.Url);
        }

        private async Task CreateResponse(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;

            await _objectResultExecutor.ExecuteAsync(
                new ActionContext
                {
                    HttpContext = context
                },
                new ObjectResult(new
                {
                    message = exception.Message
                }));
        }
    }
}