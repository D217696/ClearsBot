﻿using ClearsBot.Modules;
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
        ButtonsCommands _buttonCommands;
        Buttons _buttons;
        SlashCommands _slashCommands;
        readonly IGuilds _guilds;
        CommandService _commandService;
        IServiceProvider _serviceProvider;
        readonly Users _users;
        readonly IRaids _raids;
        readonly ILogger _logger;
        List<ApplicationCommandOptionChoiceProperties> raidOptions = new List<ApplicationCommandOptionChoiceProperties>() { new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Vault of Glass",
                                    Value = "vog"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Deep Stone Crypt",
                                    Value = "dsc"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Garden of Salvation",
                                    Value = "gos"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Crown of Sorrow",
                                    Value = "cos"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Scourge of the Past",
                                    Value = "sotp"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Last Wish",
                                    Value = "lw"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Spire of Stars",
                                    Value = "sos"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Eater of Worlds",
                                    Value = "eow"
                                },
                                new ApplicationCommandOptionChoiceProperties()
                                {
                                    Name = "Leviathan",
                                    Value = "levi"
                                }
                            };

        public DiscordEvents(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IGuilds guilds, ButtonsCommands buttonCommands, SlashCommands slashCommands, Users users, IRaids raids, ILogger logger, Buttons buttons)
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
            _client.MessageReceived += HandleCommandAsync;
            _client.InteractionCreated += InteractionCreatedAsync;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.Ready += _client_Ready;

            var buttonMethods = typeof(ButtonsCommands).GetMethods().Where(x => x.GetCustomAttribute<ButtonAttribute>() != null);
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

        private async Task<Task> _client_Ready()
        {

            //register command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "register",
            //    Description = "Register to the bot.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "membershipid",
            //                Type = ApplicationCommandOptionType.String,
            //                Description = "Enter your SteamID (joincode) or username",
            //                Required = true
            //            },
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "membershiptype",
            //                Type = ApplicationCommandOptionType.Integer,
            //                Description = "Number that represents your platform, Xbox = 1, Playstation = 2, Steam = 3, Stadia = 5"
            //            }
            //        }
            //});

            ////completions command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "completions",
            //    Description = "Gets raid completions for user.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }
            //});

            ////daily command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "daily",
            //    Description = "Gets daily raid completions for a user.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "raid",
            //                Type = ApplicationCommandOptionType.String,
            //                Description = "Specify a raid, leave empty for all raids.",
            //                Choices = raidOptions
            //            },
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }

            //});

            ////weekly command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "weekly",
            //    Description = "Gets weekly raid completions for a user.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "raid",
            //                Type = ApplicationCommandOptionType.String,
            //                Description = "Specify a raid, leave empty for all raids.",
            //                Choices = raidOptions
            //            },
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }

            //});

            ////monthly command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "monthly",
            //    Description = "Gets monthly raid completions for a user.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "raid",
            //                Type = ApplicationCommandOptionType.String,
            //                Description = "Specify a raid, leave empty for all raids.",
            //                Choices = raidOptions
            //            },
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }

            //});

            ////yearly command
            //await _client.Rest.CreateGlobalCommand(new SlashCommandCreationProperties()
            //{
            //    Name = "yearly",
            //    Description = "Gets yearly raid completions for a user.",
            //    Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "raid",
            //                Type = ApplicationCommandOptionType.String,
            //                Description = "Specify a raid, leave empty for all raids.",
            //                Choices = raidOptions
            //            },
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }

            //});


            var registerCommand = new SlashCommandBuilder()
                .WithName("register")
                .WithDescription("Register to the bot")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("membershipid")
                    .WithDescription("Bungie name or steamid")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("membershiptype")
                    .WithDescription("membership type")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("xbox", 1)
                    .AddChoice("playstation", 2)
                    .AddChoice("steam", 3)
                    .AddChoice("stadia", 5)
                );

            try
            {
                var x = await _client.Rest.CreateGuildCommand(registerCommand.Build(), 787327259752660995);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //List<SlashCommandOptionBuilder> optionBuilders = new List<SlashCommandOptionBuilder>() { } ; 
            //var createCommand = new SlashCommandBuilder()
            //      .WithName("create")
            //      .WithDescription("Create companies, groups or songs!")
            //      .AddOptions()
            //      .AddOption(new SlashCommandOptionBuilder()
            //          .WithName("company")
            //          .WithDescription("Creates a new company")
            //          .WithType(ApplicationCommandOptionType.SubCommand)
            //          .AddOption("name", ApplicationCommandOptionType.String, "The name of your company", required: true))
            //      .AddOption(new SlashCommandOptionBuilder()
            //          .WithName("group")
            //          .WithDescription("Creates a new group")
            //          .WithType(ApplicationCommandOptionType.SubCommand))
            //      .AddOption(new SlashCommandOptionBuilder()
            //          .WithName("song")
            //          .WithDescription("Creates a new song")
            //          .WithType(ApplicationCommandOptionType.SubCommand));

            //await _client.Rest.CreateGlobalCommand(createCommand.Build());
            return Task.CompletedTask;
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
                                Buttons[buttonData.CommandName].Invoke(_buttonCommands, new object[] { parsedArg });
                                break;
                            }

                            if (buttonData.DiscordUserId == parsedArg.User.Id) // if it is a private button and the user that triggered it is the same as the user that made it execute the command
                            {
                                Buttons[buttonData.CommandName].Invoke(_buttonCommands, new object[] { parsedArg });
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
