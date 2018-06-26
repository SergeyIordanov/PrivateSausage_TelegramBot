using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PrivateSausage.Web.Interfaces
{
    public interface ICallbackHandler
    {
        string CallbackType { get; }

        Task HandleAsync(Update update);
    }
}
