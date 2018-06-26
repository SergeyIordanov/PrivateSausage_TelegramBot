using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PrivateSausage.Web.Interfaces
{
    public interface ICommandHandler
    {
        string Command { get; }

        Task HandleAsync(Update update);
    }
}
