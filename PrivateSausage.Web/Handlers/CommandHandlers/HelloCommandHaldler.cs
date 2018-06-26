using System.Collections.Generic;
using System.Threading.Tasks;
using PrivateSausage.Web.Globals;
using PrivateSausage.Web.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PrivateSausage.Web.Handlers.CommandHendlers
{
    public class HelloCommandHandler : ICommandHandler
    {
        private const string ReplyText = "At your service!";

        private readonly ITelegramBotClient _botClient;

        private readonly InlineKeyboardButton[] _buttons =
        {
            new InlineKeyboardButton
            {
                Text = "At ease, private!",
                CallbackData = CallbackType.Cancel
            }
        };

        public HelloCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public string Command => CommandType.Hi;

        public Task HandleAsync(Update update)
        {
            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new List<IEnumerable<InlineKeyboardButton>> { _buttons });

            return _botClient.SendTextMessageAsync(update.Message.Chat.Id, ReplyText, replyMarkup: inlineKeyboardMarkup);
        }
    }
}
