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
        CommandHandler _handler;
        ServiceProvider _services;
        ComponentHandler _componentHandler;

        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();
        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
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

            _services = ConfigureServices();
            _client.Log += Log;
            //_client.Ready += OnClientReady;

            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("Spire of Stars is the best raid");

            _handler = _services.GetRequiredService<CommandHandler>();
            await _handler.InitializeAsync();
            _componentHandler = _services.GetRequiredService<ComponentHandler>();
            await _componentHandler.InitializeAsync();

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
                           .AddSingleton<CommandHandler>()
                           .AddSingleton<ComponentHandler>()
                           .AddSingleton<Bungie>()
                           .AddSingleton<Misc>()
                           .BuildServiceProvider();
        }
    }
}
