using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class Program
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
            _client = _services.GetRequiredService<DiscordSocketClient>();

            _commandService = new CommandService(new CommandServiceConfig
            {
                IgnoreExtraArgs = true
            });

            _client.Log += Log;

            _ = _services.GetRequiredService<Globals>();
            _handler = _services.GetRequiredService<DiscordEvents>();
            _config = _services.GetRequiredService<Config>();

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
                            .AddClearsBot()
                            .BuildServiceProvider();
        }
    }
}
