using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class EntryPoint : IHostedService
    {
        private int? _exitCode;

        DiscordSocketClient _client;
        CommandService _commandService;
        ServiceProvider _services;
        DiscordEvents _handler;
        Config _config;
        IServiceProvider _serviceProvider;

        public EntryPoint(Config config, IServiceProvider serviceProvider, DiscordSocketClient client)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_config.bot.token == "" || _config.bot.token == null) return;

            _serviceProvider.GetRequiredService<DiscordEvents>();
            _serviceProvider.GetRequiredService<UpdateLoop>();

            _client.Log += Log;

            //_handler = _services.GetRequiredService<DiscordEvents>();
            //_ = _services.GetRequiredService<UpdateLoop>();
            await _client.LoginAsync(TokenType.Bot, _config.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("Spire of Stars is the best raid");

            await Task.Delay(-1);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogDebug($"Exiting with return code: {_exitCode}");

            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
