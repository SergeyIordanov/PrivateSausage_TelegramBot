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
            var uri = FormWebhookUri(context);
            var webHookInfo = await _botClient.GetWebhookInfoAsync();

            var webhookExists = !string.IsNullOrEmpty(webHookInfo.Url);

            if (webhookExists)
            {
                if (!ValidateWebhookUri(webHookInfo, uri))
                {
                    await _botClient.DeleteWebhookAsync();

                    await CreateWebhookAsync(uri, context);
                }
            }
            else
            {
                await CreateWebhookAsync(uri, context);
            }

            await _next(context);
        }

        private async Task CreateWebhookAsync(string uri, HttpContext context)
        {
            try
            {
                await _botClient.SetWebhookAsync(
                    uri.Replace(TokenRouteVariable, _options.Token),
                    null,
                    _options.Webhook.MaxConnections,
                    _options.Webhook.AllowedUpdates);
            }
            catch (ApiRequestException exception) when (exception.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                await CreateResponse(context, exception, (int)HttpStatusCode.BadRequest);
            }
        }

        private bool ValidateWebhookUri(WebhookInfo webHookInfo, string requiredUri)
        {
            return string.Equals(webHookInfo.Url, requiredUri);
        }

        private string FormWebhookUri(HttpContext context)
        {
            var host = new Uri(context.Request.Host.ToString(), UriKind.Absolute);
            var api = new Uri(_options.Webhook.ApiUrl, UriKind.Relative);
            var uri = host + "/" + api;

            return uri;
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