using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ClearsBot
{
    public static class Setup
    {
        public static IServiceCollection AddClearsBot(this IServiceCollection services)
        {
            DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All
            });

            CommandService commandService = new CommandService(new CommandServiceConfig
            {
                IgnoreExtraArgs = true
            });

            return services
            .AddHostedService<EntryPoint>()
            .AddSingleton(client)
            .AddSingleton(serviceProvider => 
            {
                var commandService = new CommandService(new CommandServiceConfig()
                {
                    IgnoreExtraArgs = true
                });

                commandService.AddModuleAsync(typeof(TextCommands), serviceProvider);

                return commandService;
            })
            .AddSingleton<DiscordEvents>()
            .AddSingleton<UpdateLoop>()
            .AddSingleton<IBungie, Bungie>()
            .AddSingleton<TextCommands>()
            .AddSingleton<Users>()
            .AddSingleton<IBungieDestiny2RequestHandler, BungieDestiny2RequestHandler>()
            .AddSingleton<ILogger, Logger>()
            .AddSingleton<IGuilds, Guilds>()
            .AddSingleton<IPermissions, Permissions>()
            .AddSingleton<ButtonsCommands>()
            .AddSingleton<IFormatting, Formatting>()
            .AddSingleton<Commands>()
            .AddSingleton<IRaids, Raids>()
            .AddSingleton<IStorage, Storage>()
            .AddSingleton<Roles>()
            .AddSingleton<SlashCommands>()
            .AddSingleton<Completions>()
            .AddSingleton<MessageTracking>()
            .AddSingleton<IFormatting, Formatting>()
            .AddSingleton<ILanguages, Languages>()
            .AddSingleton<Buttons>()
            .AddSingleton<ISlashes, Slashes>()
            .AddSingleton<Config>();
        }
    }
}
