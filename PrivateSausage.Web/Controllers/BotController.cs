using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PrivateSausage.Web.Controllers
{
    [Route("api/bot/{token}")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        public BotController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [HttpPost("updates")]
        public async Task<ActionResult<string>> Post([FromBody] IEnumerable<Update> updates)
        {
            var result = string.Empty;

            foreach (var update in updates)
            {
                var commandEntity = update.Message.Entities?.FirstOrDefault(entity => entity.Type == MessageEntityType.BotCommand);

                if (commandEntity != null)
                {
                    var commandText = update.Message.Text.Substring(commandEntity.Offset, commandEntity.Length);

                    if (commandText.Equals("/hi"))
                    {
                        await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sir! Yes, sir!");
                    }
                }
            }


            return result;
        }
    }
}
