using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrivateSausage.Web.Configuration;
using PrivateSausage.Web.Filters;
using PrivateSausage.Web.Handlers.CallbackHandlers;
using PrivateSausage.Web.Handlers.CommandHendlers;
using PrivateSausage.Web.Interfaces;
using PrivateSausage.Web.Middlewares;
using Telegram.Bot;

namespace PrivateSausage.Web
{
    public class Startup
    {
        private const string BotConfigurationSectionName = "Bot";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var botOptions = new BotOptions();
            Configuration.Bind(BotConfigurationSectionName, botOptions);

            services.Configure<BotOptions>(Configuration.GetSection(BotConfigurationSectionName));

            services.AddMvc(options =>
                {
                    options.Filters.Add<BotTokenValidationFilterAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botOptions.Token));

            RegisterHandlers(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseWebhook();
        }

        private static void RegisterHandlers(IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, HelloCommandHandler>();

            services.AddSingleton<ICallbackHandler, CancelCallbackHandler>();
            services.AddSingleton<ICallbackHandler, ReportCallbackHandler>();
        }
    }
}
