using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace PrivateSausage.Web.Configuration
{
    public class WebhookOptions
    {
        public string Url { get; set; }

        public int MaxConnections { get; set; }

        public IEnumerable<UpdateType> AllowedUpdates { get; set; }
    }
}