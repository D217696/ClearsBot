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
            ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Day, "Daily").Build());
        }

        [SlashCommand("weekly")]
        public async Task WeeklySlashCommand(SocketSlashCommand command)
        {
            ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Week, "Weekly").Build());
        }

        [SlashCommand("monthly")]
        public async Task MonthlySlashCommand(SocketSlashCommand command)
        {
            ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Month, "Monthly").Build());
        }

        [SlashCommand("yearly")]
        public async Task YearlySlashCommand(SocketSlashCommand command)
        {
            ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;
            string raidStringDaily = command.Data.Options == null ? "" : command.Data.Options.Where(x => x.Name == "raid").FirstOrDefault().Value.ToString();
            await command.FollowupAsync(embed: _commands.TimeFrameCommand(userId, guildId, raidStringDaily, TimeFrameHours.Year, "Yearly").Build());
        }

        [SlashCommand("completions")]
        public async Task CompletionsSlashCommand(SocketSlashCommand command)
        {
            ulong userId = command.Data.Options == null ? command.User.Id : command.Data.Options.Where(x => x.Name == "user").FirstOrDefault() == null ? command.User.Id : ((IGuildUser)command.Data.Options.Where(x => x.Name == "user").FirstOrDefault().Value).Id;
            ulong guildId = ((SocketGuildChannel)command.Channel).Guild.Id;

            List<User> users = _users.GetUsers(guildId, userId);
            if (users.Count == 0)
            {
                await command.FollowupAsync("You have not registered.", false, null, InteractionResponseType.ChannelMessageWithSource, false, null, null, null);
                return;
            }

            await command.FollowupAsync(embed: _utilities.GetCompletionsForUser(users.FirstOrDefault(), guildId).Build(), component: _utilities.GetButtonsForUser(users, guildId, "completions", users.FirstOrDefault()).Build());

        }

        [SlashCommand("register")]
        public async Task RegisterSlashCommand(SocketSlashCommand command) 
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Register");
            embed.WithDescription("Forwarding message...");
            var restFollowupMessage = await command.FollowupAsync("", false, new[] { embed.Build() });
            await _commands.RegisterUserCommand(command.Channel, ((SocketGuildChannel)command.Channel).Guild.Id, command.User.Id, command.User.Username, command.Data.Options.Where(x => x.Name == "membershipid").FirstOrDefault() == null ? "" : command.Data.Options.Where(x => x.Name == "membershipid").FirstOrDefault().Value.ToString(), command.Data.Options.Where(x => x.Name == "membershiptype").FirstOrDefault() == null ? "" : command.Data.Options.Where(x => x.Name == "membershiptype").FirstOrDefault().Value.ToString(), restFollowupMessage);
        }
    }
}
