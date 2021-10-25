using ClearsBot.Modules;
using ClearsBot.Modules.DiscordInterfaces;
using ClearsBot.Modules.Logger;
using ClearsBot.Modules.Raids;
using ClearsBot.Modules.Slashes;
using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class DiscordEvents
    {
        DiscordSocketClient _client;
        Dictionary<string, MethodInfo> ButtonCommands = new Dictionary<string, MethodInfo>();
        Dictionary<string, MethodInfo> SlashCommands = new Dictionary<string, MethodInfo>();
        ButtonsCommands _buttonCommands;
        Buttons _buttons;
        SlashCommands _slashCommands;
        readonly IGuilds _guilds;
        CommandService _commandService;
        IServiceProvider _serviceProvider;
        readonly Users _users;
        readonly IRaids _raids;
        readonly ILogger _logger;
        readonly ISlashes _slashes;

        public DiscordEvents(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IGuilds guilds, ButtonsCommands buttonCommands, SlashCommands slashCommands, Users users, IRaids raids, ILogger logger, Buttons buttons, ISlashes slashes)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _guilds = guilds;
            _buttonCommands = buttonCommands;
            _slashCommands = slashCommands;
            _users = users;
            _raids = raids;
            _logger = logger;
            _buttons = buttons;
            _slashes = slashes;
            _client.MessageReceived += HandleCommandAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.LeftGuild += _client_LeftGuild;
            _client.Ready += _client_Ready;

            var buttonMethods = typeof(ButtonsCommands).GetMethods().Where(x => x.GetCustomAttribute<ButtonAttribute>() != null);
            var slashMethods = typeof(SlashCommands).GetMethods().Where(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);

            foreach (MethodInfo methodInfo in buttonMethods)
            {
                ButtonCommands.Add(methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(ButtonAttribute)).FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString().ToLower(), methodInfo);
            }

            foreach (MethodInfo methodInfo in slashMethods)
            {
                SlashCommands.Add(methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(SlashCommandAttribute)).FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString().ToLower(), methodInfo);
            }

            Init();
        }

        private Task _client_LeftGuild(SocketGuild arg)
        {
            _guilds.LeftGuild(arg.Id);
            return Task.CompletedTask;
        }

        private async Task<Task> _client_Ready()
        {
            foreach(SocketGuild guild in _client.Guilds)
            {
                if (_guilds.GetGuild(guild.Id) == null)
                {
                    _guilds.JoinedGuild(guild.Id, guild.OwnerId);
                }

                InternalGuild interalGuild = _guilds.GetGuild(guild.Id);
                if (interalGuild.IsActive && interalGuild.UsesSlashCommands)
                {
                    _ = _slashes.RegisterSlashCommandsForGuild(guild.Id);
                }
            }
            return Task.CompletedTask;
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            _guilds.JoinedGuild(arg.Id, arg.OwnerId);
            await _users.SyncUsers(arg.Id);
            _raids.GuildJoined(arg.Id);
        }


        private async void Init()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        }

        public async Task InteractionCreatedAsync(SocketInteraction arg)
        {
            switch (arg.Type)
            {
                case InteractionType.MessageComponent:
                    var parsedArg = (SocketMessageComponent)arg;
                    switch (parsedArg.Data.Type)
                    {
                        case ComponentType.Button:
                            ButtonData buttonData = _buttons.GetButtonData(parsedArg.Data.CustomId);
                            if (buttonData == null) break;
                            if (!buttonData.PrivateButton) // if its NOT a private button execute the command
                            {
                                ButtonCommands[buttonData.CommandName].Invoke(_buttonCommands, new object[] { parsedArg });
                                break;
                            }

                            if (buttonData.DiscordUserId == parsedArg.User.Id) // if it is a private button and the user that triggered it is the same as the user that made it execute the command
                            {
                                ButtonCommands[buttonData.CommandName].Invoke(_buttonCommands, new object[] { parsedArg });
                            }
                            break;
                    }
                    break;
                case InteractionType.ApplicationCommand:
                    if (arg is SocketSlashCommand command)
                    {
                        if (SlashCommands.ContainsKey(command.CommandName.ToLower()))
                        {
                            SlashCommands[command.CommandName.ToLower()].Invoke(_slashCommands, new object[] { command });
                        }
                        else
                        {
                            _logger.LogError($"No slash command logic found for {command.CommandName}");
                        }
                    }
                break;
            }
        }
        public async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;

            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;
            if (context.Guild == null) return;

            int argPos = 0;
            if (msg.HasStringPrefix(_guilds.GetGuild(context.Guild.Id).Prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider, MultiMatchHandling.Best);
                if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    Console.WriteLine(result.ErrorReason);
                }

                Console.WriteLine($"{msg} | {msg.Author.Username}#{msg.Author.Discriminator}");
            }
        }
    }
}
