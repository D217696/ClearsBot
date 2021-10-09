using ClearsBot.Modules;
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
    class CommandHandler
    {
        readonly IGuilds _guilds;
        DiscordSocketClient _client;
        CommandService _commandService;
        IServiceProvider _serviceProvider;

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
        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IGuilds guilds)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _guilds = guilds;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
            _client.MessageReceived += HandleCommandAsync;
            _client.Ready += _client_Ready;
        }

        public async Task<Task> _client_Ready()
        {
            //await Guilds.Initialize();
            //await Users.Initialize();
            await Languages.Initialize();

            //foreach (SocketGuild guild in _client.Guilds)
            //{
            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties()
            //    {
            //        Name = "register",
            //        Description = "Register to the bot.",
            //        Options = new List<ApplicationCommandOptionProperties>()
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
            //    }, guild.Id);

            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties() {
            //        Name = "completions",
            //        Description = "Gets raid completions for user.",
            //        Options = new List<ApplicationCommandOptionProperties>()
            //        {
            //            new ApplicationCommandOptionProperties()
            //            {
            //                Name = "user",
            //                Type = ApplicationCommandOptionType.User,
            //                Description = "User to get completions for."
            //            }
            //        }
            //    }, guild.Id);

            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties() { 
            //        Name = "daily", 
            //        Description = "Gets daily raid completions for a user.",
            //        Options = new List<ApplicationCommandOptionProperties>()
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
                
            //    }, guild.Id);

            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties()
            //    {
            //        Name = "weekly",
            //        Description = "Gets weekly raid completions for a user.",
            //        Options = new List<ApplicationCommandOptionProperties>()
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

            //    }, guild.Id); 
                
            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties()
            //    {
            //        Name = "monthly",
            //        Description = "Gets monthly raid completions for a user.",
            //        Options = new List<ApplicationCommandOptionProperties>()
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

            //    }, guild.Id);

            //    await _client.Rest.CreateGuildCommand(new SlashCommandCreationProperties()
            //    {
            //        Name = "yearly",
            //        Description = "Gets yearly raid completions for a user.",
            //        Options = new List<ApplicationCommandOptionProperties>()
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

            //    }, guild.Id);
            //}

            return Task.CompletedTask;
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
                else { 
                    Console.WriteLine(result.ErrorReason);
                }

                Console.WriteLine($"{msg} | {msg.Author.Username}#{msg.Author.Discriminator}");
            }
        }
    }
}
