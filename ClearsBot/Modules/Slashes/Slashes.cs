using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Slashes : ISlashes
    {
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
        public async Task RegisterSlashCommandsForGuild(ulong guildId)
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


            //var registerCommand = new SlashCommandBuilder()
            //    .WithName("register")
            //    .WithDescription("Register to the bot")
            //    .AddOption(new SlashCommandOptionBuilder()
            //        .WithName("membershipid")
            //        .WithDescription("Bungie name or steamid")
            //        .WithType(ApplicationCommandOptionType.String)
            //        .WithRequired(true)
            //    )
            //    .AddOption(new SlashCommandOptionBuilder()
            //        .WithName("membershiptype")
            //        .WithDescription("membership type")
            //        .WithType(ApplicationCommandOptionType.Integer)
            //        .AddChoice("xbox", 1)
            //        .AddChoice("playstation", 2)
            //        .AddChoice("steam", 3)
            //        .AddChoice("stadia", 5)
            //    );

            //try
            //{
            //    var x = await Program._client.Rest.CreateGuildCommand(registerCommand.Build(), 787327259752660995);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}


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
        }
    }
}
