﻿using ClearsBot.Objects;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Commands
    {
        readonly IBungie _bungie;
        readonly ILeaderboards _leaderboards;
        readonly Users _users;
        readonly IRaids _raids;
        readonly IPermissions _permissions;
        readonly IUtilities _utilities;
        public Commands(IBungie bungie, ILeaderboards leaderboards, Users users, IRaids raids, IPermissions permissions, IUtilities utilities)
        {
            _bungie = bungie;
            _leaderboards = leaderboards;
            _users = users;
            _raids = raids;
            _permissions = permissions;
            _utilities = utilities;
        }

        //leaderboard commands
        public EmbedBuilder TimeFrameCommand(ulong userId, ulong guildId, string raidString, TimeFrameHours timeFrame, string commandSyntax)
        {
            var embed = new EmbedBuilder();

            int currentTimeFrame = Convert.ToInt32(Math.Floor((DateTime.UtcNow - _bungie.ReleaseDate).TotalHours / (int)timeFrame));
            DateTime startDate = _bungie.ReleaseDate.AddHours(currentTimeFrame * (int)timeFrame);
            DateTime endDate = _bungie.ReleaseDate.AddHours((currentTimeFrame + 1) * (int)timeFrame);
            if (timeFrame == TimeFrameHours.Month)
            {
                currentTimeFrame = Convert.ToInt32(DateTime.UtcNow.ToString("yyyyMM"));
                startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            Raid raid = _raids.GetRaid(guildId, raidString);
            if (raid == null)
            {
                embed.WithTitle($"{commandSyntax} raid completions from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"); 
            }
            else
            {
                embed.WithTitle($"{commandSyntax} {raid.DisplayName} completions from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
            }

            IEnumerable<(User user, int completions, int rank)> usersWithCompletionsAndRank = _leaderboards.GetUserCompetionsByTimeframe(_users.GetGuildUsers(guildId).Where(x => x.Completions.Count > 0), raid, timeFrame, currentTimeFrame);
            IEnumerable<(User user, int completions, int rank)> usersWithMaxCompletionsAndRank = _leaderboards.GetUserCompetionsMaxByTimeframe(_users.GetGuildUsers(guildId).Where(x => x.Completions.Count > 0), raid, timeFrame);
            embed.AddField($"{commandSyntax} raid completions", _leaderboards.CreateLeaderboardString(usersWithCompletionsAndRank, userId, 10, true), true);
            embed.AddField($"{commandSyntax} raid completion leaderboard", _leaderboards.CreateLeaderboardString(usersWithMaxCompletionsAndRank, userId), true);
            return embed;
        }
        public EmbedBuilder RankCommand(ulong userId, ulong guildId, string raidString)
        {
            Raid raid = _raids.GetRaid(guildId, raidString);
            IEnumerable<(User user, int completions, int rank)> usersWithCompletions = _users.GetListOfUsersWithCompletions(guildId, _bungie.ReleaseDate, DateTime.Now, raid);

            var embed = new EmbedBuilder();

            if (raid != null)
            {
                embed.WithTitle($"Top 10 {raid.DisplayName} completions");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
            }
            else
            {
                embed.WithTitle("Top 10 raid completions");
            }
            embed.WithDescription(_leaderboards.CreateLeaderboardString(usersWithCompletions, userId, 10, true));
            return null;
        }
        //raid commands
        public string EditRaidTimeCommand(IGuildUser guildUser, ulong guildId, string raidString, string timeString)
        {
            if (_permissions.GetPermissionForUser(guildUser) < PermissionLevels.AdminRole) return "No permission";

            if (timeString.Length == 5 && timeString.Contains(":"))
            {
                timeString = "00:" + timeString;
            }

            bool parseSucceeded = TimeSpan.TryParse(timeString, out TimeSpan completionTime);
            if (!parseSucceeded) return "Could not convert time to TimeSpan format: hh:mm:ss";

            Raid raid = _raids.SetRaidTime(guildId, raidString, completionTime);
            if (raid == null) return "No raid found";

            return $"{raid.DisplayName} completion time set to: {raid.CompletionTime}";
        }
        public string AddRaidShortcutCommand(IGuildUser guildUser, ulong guildId, string raidString, string shortcut)
        {
            if (_permissions.GetPermissionForUser(guildUser) < PermissionLevels.AdminRole) return "No permission";

            Raid raid = _raids.AddShortcut(guildId, raidString, shortcut);
            if (raid == null) return "No raid found";

            return $"Added shortcut: {shortcut} to {raid.DisplayName}";
        }
        public EmbedBuilder DisplayRaidsCommand(ulong guildId)
        {
            var embed = new EmbedBuilder();
            foreach (Raid raid in _raids.GetRaids(guildId))
            {
                string value = "**Shortcuts**\n";
                foreach (string shortcut in raid.Shortcuts)
                {
                    value += shortcut + "\n";
                }
                embed.AddField(raid.DisplayName, value, true);
            }
            return embed;
        }
        //user commands
        public async Task RegisterUserCommand(ISocketMessageChannel channel, ulong guildId, ulong userId, string discordUsername, string membershipId = "", string membershipType = "", RestFollowupMessage restFollowupMessage = null)
        {
            if (membershipId == "")
            {
                await channel.SendMessageAsync("Usage: /register (Steam Id | Membership Id) (Membership Type)");
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithDescription("Getting user...");
            embed.WithTitle("Registering " + discordUsername);
            RestUserMessage restUserMessage = null;
            if (restFollowupMessage == null)
            {
                restUserMessage = await channel.SendMessageAsync(embed: embed.Build());
            }
            else
            {
                restUserMessage = restFollowupMessage;
                await restUserMessage.ModifyAsync(x => x.Embed = embed.Build());
            }

            RequestData requestData = await _bungie.GetRequestDataAsync(membershipId, membershipType);
            if (requestData.Code != 1 && requestData.Code != 8)
            {
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription(_utilities.GetErrorCodeForUserSearch(requestData)).Build());
                return;
            }
            else if (requestData.Code == 8)
            {
                int buttonCount = 0;
                int buttonRow = 0;
                var componentBuilder = new ComponentBuilder();
                foreach (UserInfoCard userInfoCard in requestData.profiles)
                {
                    ButtonStyle buttonStyle = _utilities.GetButtonStyleForPlatform(userInfoCard.MembershipType);

                    componentBuilder.WithButton(new ButtonBuilder().WithLabel(userInfoCard.DisplayName).WithCustomId($"register_{userInfoCard.MembershipId}_{userInfoCard.MembershipType}").WithStyle(buttonStyle), buttonRow);
                    buttonCount++;
                    if (buttonCount % 5 == 0) buttonRow++;
                }
                await restUserMessage.ModifyAsync(x => x.Components = componentBuilder.Build());
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("Multiple profiles found, select correct one below").Build());
                return;
            }

            UserResponse userResponse = await _users.CreateAndAddUser(guildId, userId, requestData);
            switch (userResponse.Code)
            {
                case 1:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("User created. - Getting Activities.").Build());
                    break;
                //case 2:
                //    await restUserMessage.ModifyAsync(x => x.Content = "You already have an account linked to your discord.");
                //    break;
                case 3:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("There's already a discord account linked to this profile.").Build());

                    return;
                case 4:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("There's already a discord account linked to this profile.").Build());
                    return;
            }

            userResponse.User.DateLastPlayed = new DateTime(2017, 08, 06, 0, 0, 0);
            GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(userResponse.User);
            switch (getCompletionsResponse.Code)
            {
                case 1:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Activities found: {_users.GetGuildUsers(guildId).Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.DiscordID == userId && x.MembershipId == requestData.MembershipId).FirstOrDefault().Completions.Count} raid completions").Build());
                    break;
                default:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Something went wrong: {getCompletionsResponse.ErrorMessage}").Build());
                    break;
            }
        }
    }
}