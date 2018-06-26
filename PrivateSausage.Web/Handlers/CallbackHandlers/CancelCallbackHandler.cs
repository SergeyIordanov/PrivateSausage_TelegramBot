using System.Threading.Tasks;
using PrivateSausage.Web.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PrivateSausage.Web.Handlers.CallbackHandlers
{
    public class CancelCallbackHandler : ICallbackHandler
    {
        private readonly ITelegramBotClient _botClient;

        public CancelCallbackHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public string CallbackType => Globals.CallbackType.Cancel;

        public Task HandleAsync(Update update)
        {
            return _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Sir! Yes, sir!");
        }
    }
}
