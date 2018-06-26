using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PrivateSausage.Web.Extensions
{
    public static class UpdateExtensions
    {
        public static IEnumerable<string> GetCommands(this Update update, bool distinct)
        {
            if (update.Type != UpdateType.Message)
            {
                return Enumerable.Empty<string>();
            }

            var commandEntities = update.Message.Entities.Where(entity => entity.Type == MessageEntityType.BotCommand);

            var commands = commandEntities.Select(entity => update.Message.Text.Substring(entity.Offset, entity.Length));

            if (distinct)
            {
                commands = commands.Distinct();
            }

            return commands;
        }
    }
}
