using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ClearsBot
{

    class Program
    {
        public static DiscordSocketClient _client;
        CommandService _commandService;
        ServiceProvider _services;
        Config _config;
        DiscordEvents _handler;
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();
        public async Task StartAsync()
        {
            _services = ConfigureServices();
            _config = _services.GetRequiredService<Config>();

            if (_config.bot.token == "" || _config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All
            });

            _commandService = new CommandService(new CommandServiceConfig
            {
                IgnoreExtraArgs = true
            });

            _client.Log += Log;

            _handler = _services.GetRequiredService<DiscordEvents>();
            _ = _services.GetRequiredService<UpdateLoop>();
            await _client.LoginAsync(TokenType.Bot, _config.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("Spire of Stars is the best raid");

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                           .AddSingleton(_client)
                           .AddSingleton(_commandService)
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
                           .AddSingleton<IGuilds, Guilds>()
                           .BuildServiceProvider();
        }
    }
}
