using ClearsBot.Modules;
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

namespace ClearsBot.Modules
{
    public class DiscordEvents
    {
        DiscordSocketClient _client;
        Dictionary<string, MethodInfo> Buttons = new Dictionary<string, MethodInfo>();
        Dictionary<string, MethodInfo> SlashCommands = new Dictionary<string, MethodInfo>();
        Buttons _buttons;
        SlashCommands _slashCommands;
        readonly IGuilds _guilds;
        CommandService _commandService;
        IServiceProvider _serviceProvider;
        readonly Users _users;
        readonly IRaids _raids;

        public DiscordEvents(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IGuilds guilds, Buttons buttons, SlashCommands slashCommands, Users users, IRaids raids)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _guilds = guilds;
            _buttons = buttons;
            _slashCommands = slashCommands;
            _users = users;
            _raids = raids;
            _client.MessageReceived += HandleCommandAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
            _client.JoinedGuild += _client_JoinedGuild;

            var buttonMethods = typeof(Buttons).GetMethods().Where(x => x.GetCustomAttribute<ButtonAttribute>() != null);
            var slashMethods = typeof(SlashCommands).GetMethods().Where(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);

            foreach (MethodInfo methodInfo in buttonMethods)
            {
                Buttons.Add(methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(ButtonAttribute)).FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString().ToLower(), methodInfo);
            }
            foreach (MethodInfo methodInfo in slashMethods)
            {
                SlashCommands.Add(methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(SlashCommandAttribute)).FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString().ToLower(), methodInfo);
            }

            Init();
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            _guilds.GuildJoined(arg.Id, arg.OwnerId);
            await _users.GuildJoined(arg.Id);
            _raids.GuildJoined(arg.Id);
        }

        private async void Init()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "test",
            //    Description = "test command",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //    {
            //        new ApplicationCommandOptionProperties()
            //        {
            //            Choices = new List<ApplicationCommandOptionChoiceProperties>()
            //            {
            //                new ApplicationCommandOptionChoiceProperties()
            //                {
            //                    Name = "testoptionchoice",
            //                    Value = "testoptionchoice"
            //                }
            //            },
            //            Name = "raid", 
            //            Description = "raid description",
            //            Type = ApplicationCommandOptionType.String,
            //            Required = false
            //        }
            //    }
            //});
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
                            string[] parameters = parsedArg.Data.CustomId.Split("_");

                            Buttons[parameters[0]].Invoke(_buttons, new object[] { parsedArg });
                            break;
                    }
                    break;
                case InteractionType.ApplicationCommand:
                    if (arg is SocketSlashCommand command)
                    {
                        SlashCommands[command.Data.Name.ToLower()].Invoke(_slashCommands, new object[] { command.Data });
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
