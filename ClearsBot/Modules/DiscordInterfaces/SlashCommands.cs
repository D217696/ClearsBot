using ClearsBot.Objects;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class SlashCommands
    {
        readonly Users _users;
        readonly Commands _commands;
        readonly IUtilities _utilities;
        public SlashCommands(Users users, Commands commands, IUtilities utilities)
        {
            _users = users;
            _commands = commands;
            _utilities = utilities;
        }

        [SlashCommand("daily")]
        public async Task DailySlashCommand(SocketSlashCommand command)
        {
            SocketSlashCommandData commandData = (SocketSlashCommandData)command.Data;
            ulong userId = commandData == null ? command.User.Id : commandData.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser) commandData.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = commandData.Options == null ? "" : commandData.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Day, "Daily").Build());
        }

        [SlashCommand("weekly")]
        public async Task WeeklySlashCommand(SocketSlashCommand command)
        {
            SocketSlashCommandData commandData = (SocketSlashCommandData)command.Data;
            ulong userId = commandData.Options == null ? command.User.Id : commandData.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser) commandData.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = commandData.Options == null ? "" : commandData.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Week, "Weekly").Build());
        }

        [SlashCommand("monthly")]
        public async Task MonthlySlashCommand(SocketSlashCommand command)
        {
            SocketSlashCommandData commandData = (SocketSlashCommandData)command.Data;
            ulong userId = commandData.Options == null ? command.User.Id : commandData.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)commandData.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = commandData.Options == null ? "" : commandData.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Month, "Monthly").Build());
        }

        [SlashCommand("yearly")]
        public async Task YearlySlashCommand(SocketSlashCommand command)
        {
            SocketSlashCommandData commandData = (SocketSlashCommandData)command.Data;
            ulong userId = commandData.Options == null ? command.User.Id : commandData.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)commandData.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = commandData.Options == null ? "" : commandData.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Year, "Yearly").Build());
        }

        [SlashCommand("completions")]
        public async Task CompletionsSlashCommand(SocketSlashCommand command)
        {
            SocketSlashCommandData commandData = (SocketSlashCommandData)command.Data;
            ulong userId = commandData.Options == null ? command.User.Id : commandData.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)commandData.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;

            List<User> users = _users.GetUsers(guildId, userId);
            if (users.Count == 0)
            {
                await command.FollowupAsync("You have not registered.");
                return;
            }

            await command.FollowupAsync(embed: _utilities.GetCompletionsForUser(users.FirstOrDefault(), guildId).Build(), component: _utilities.GetButtonsForUser(users, guildId, "completions", users.FirstOrDefault()).Build());

        }

        [SlashCommand("register")]
        public async Task RegisterSlashCommand(SocketSlashCommand command)
        {
            if (command.Data is SocketSlashCommandData commandData)
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Register");
                embed.WithDescription("Forwarding message...");
                var restFollowupMessage = await command.FollowupAsync("test");
                string membershipId = commandData.Options.Where(x => x.Name == "membershipid").FirstOrDefault() == null ? "" : commandData.Options.Where(x => x.Name == "membershipid").FirstOrDefault().Value.ToString();
                string membershipType = commandData.Options.Where(x => x.Name == "membershiptype").FirstOrDefault() == null ? "" : commandData.Options.Where(x => x.Name == "membershiptype").FirstOrDefault().Value.ToString();
                await _commands.RegisterUserCommand(command.Channel, ((SocketGuildChannel)command.Channel).Guild.Id, command.User.Id, command.User.Username, membershipId, membershipType, restFollowupMessage);
            }
        }
    }
}
