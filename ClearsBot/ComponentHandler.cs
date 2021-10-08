using ClearsBot.Modules;
using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class ComponentHandler
    {
        DiscordSocketClient _client;
        CommandService _commandService;
        IServiceProvider _serviceProvider;
        IUtilities _utilities;
        ILeaderboards _leaderboards;
        Users _users;
        Dictionary<string, MethodInfo> Buttons = new Dictionary<string, MethodInfo>();
        Buttons _buttons;
        object buttonClassObject = null;

        public ComponentHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IUtilities utilities, Users users, Buttons buttons, ILeaderboards leaderboards)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _utilities = utilities;
            _users = users;
            _buttons = buttons;
            _leaderboards = leaderboards;

            _client.InteractionCreated += InteractionCreatedAsync;
            var methods = typeof(Buttons).GetMethods().Where(x => x.GetCustomAttribute<ButtonAttribute>() != null);

            foreach (MethodInfo methodInfo in methods)
            {
                Buttons.Add(methodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(ButtonAttribute)).FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString().ToLower(), methodInfo);
            }
        }

        public async Task InteractionCreatedAsync(SocketInteraction arg)
        {
            switch (arg.Type)
            {
                case InteractionType.MessageComponent:
                    var parsedArg = (SocketMessageComponent) arg;
                    switch (parsedArg.Data.Type)
                    {
                        case ComponentType.Button:
                            string[] parameters = parsedArg.Data.CustomId.Split("_");

                            Buttons[parameters[0]].Invoke(_buttons, new object[] { parsedArg });
                            break;
                    }
                    break;
                case InteractionType.ApplicationCommand:
                    if(arg is SocketSlashCommand command)
                    {
                        ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
                        ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;

                        switch (command.Data.Name)
                        {
                            case "register":
                                var embed = new EmbedBuilder();
                                embed.WithTitle("Register");
                                embed.WithDescription("Forwarding message..."); 
                                var restFollowupMessage = await command.FollowupAsync("", false, new[] { embed.Build() });
                                await _users.RegisterUser(command.Channel, ((SocketGuildChannel) command.Channel).Guild.Id, command.User.Id, command.User.Username, command.Data.Options.Where(x => x.Name == "membershipid").FirstOrDefault() == null ? "" : command.Data.Options.Where(x => x.Name == "membershipid").FirstOrDefault().Value.ToString(), command.Data.Options.Where(x => x.Name == "membershiptype").FirstOrDefault() == null ? "" : command.Data.Options.Where(x => x.Name == "membershiptype").FirstOrDefault().Value.ToString(), restFollowupMessage);
                                break;
                            case "completions":
                                List<User> users = _users.GetUsers(guildId, userId);
                                var x = users.Count == 0 ? (await command.FollowupAsync("", false, new[] { new EmbedBuilder { Title = "Completions", Description = "No users found." }.Build() })) : (await command.FollowupAsync("", false, new[] { _utilities.GetCompletionsForUser(users.FirstOrDefault(), guildId).Build() }, component: _utilities.GetButtonsForUser(users, guildId, "completions", users.FirstOrDefault()).Build()));
                                break;
                            case "daily":
                                string raidStringDaily = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
                                DateTime startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 17, 0, 0);
                                startDate = DateTime.UtcNow.TimeOfDay < new TimeSpan(17, 0, 0) ? startDate.AddDays(-1) : startDate;
                             //   await command.FollowupAsync(embed: _leaderboards.CreateLeaderboardMessage(0, 1, raidStringDaily, guildId, userId, (int)TimeFrameHours.Day, "today", "10", startDate).Build());
                                break;
                            case "weekly":
                                string raidStringWeekly = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
                           
                               // await command.FollowupAsync(embed: _leaderboards.CreateLeaderboardMessage(0, 7, raidStringWeekly, guildId, userId, (int)TimeFrameHours.Week, "this week", "10").Build());
                                break;
                            case "monthly":
                                string raidStringMonthly = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
                                //await command.FollowupAsync(embed: _leaderboards.CreateLeaderboardMessage(-21, 28, raidStringMonthly, guildId, userId, (int)TimeFrameHours.Month, "this month", "10").Build());
                                break;
                            case "yearly":
                                string raidStringYearly = command.Data.Options == null ? "" : command.Data.Options.FirstOrDefault(x => x.Name == "raid").Value.ToString();
                               // await command.FollowupAsync(embed: _leaderboards.CreateLeaderboardMessage(-358, 365, raidStringYearly, guildId, userId, (int)TimeFrameHours.Year, "this year", "10").Build());
                                break;
                        }
                    }
                    break;
            }
        }

       
    }
}
