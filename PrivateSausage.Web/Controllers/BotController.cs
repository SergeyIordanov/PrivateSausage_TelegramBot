using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivateSausage.Web.Extensions;
using PrivateSausage.Web.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PrivateSausage.Web.Controllers
{
    [Route("api/bot/{token}")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IEnumerable<ICommandHandler> _commandHandlers;
        private readonly IEnumerable<ICallbackHandler> _callbackHandlers;

        public BotController(
            IEnumerable<ICommandHandler> commandHandlers,
            IEnumerable<ICallbackHandler> callbackHandlers)
        {
            _commandHandlers = commandHandlers;
            _callbackHandlers = callbackHandlers;
        }

        [HttpPost("updates")]
        public async Task<ActionResult> Post([FromBody] Update update)
        {
            ProcessCommands(update);
            ProcessCallback(update);

            return Ok();
        }

        private void ProcessCallback(Update update)
        {
            if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackHandler = _callbackHandlers
                    .FirstOrDefault(handler => handler.CallbackType.Equals(update.CallbackQuery.Data));
                callbackHandler?.HandleAsync(update);
            }
        }

        private void ProcessCommands(Update update)
        {
            var commands = update.GetCommands(true);

            foreach (var command in commands)
            {
                var commandHandler = _commandHandlers.FirstOrDefault(handler => handler.Command.Equals(command));
                commandHandler?.HandleAsync(update);
            }
        }
    }
}
