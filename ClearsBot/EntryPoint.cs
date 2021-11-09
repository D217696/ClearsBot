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
        DiscordEvents _discordEvents;
        UpdateLoop _updateLoop;
        Config _config;
        Globals _globals;

        public EntryPoint(Config config, DiscordSocketClient client, DiscordEvents discordEvents, UpdateLoop updateLoop, Globals globlas)
        {
            _config = config;
            _client = client;
            _discordEvents = discordEvents;
            _updateLoop = updateLoop;
            _globals = globlas;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_config.bot.token == "" || _config.bot.token == null) return;

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _config.bot.token);
            await _client.StartAsync();
            await _client.SetGameAsync("Spire of Stars is the best raid");
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
