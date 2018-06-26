using System;
using System.Text;
using System.Threading.Tasks;
using PrivateSausage.Web.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PrivateSausage.Web.Handlers.CallbackHandlers
{
    public class ReportCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;

        public ReportCallbackHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public string CallbackType => Globals.CallbackType.Report;

        public async Task HandleAsync(Update update)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Report");
            messageBuilder.AppendLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm TT"));
            messageBuilder.AppendLine("-------------------");
            messageBuilder.AppendLine("Nothing to report.");

            await _botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, messageBuilder.ToString());

            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
}
