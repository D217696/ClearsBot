using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Modules
{
    public class Commands
    {
        readonly IBungie _bungie;
        readonly ILeaderboards _leaderboards;
        readonly Users _users;
        readonly IRaids _raids;
        public Commands(IBungie bungie, ILeaderboards leaderboards, Users users, IRaids raids)
        {
            _bungie = bungie;
            _leaderboards = leaderboards;
            _users = users;
            _raids = raids;
        }

        public EmbedBuilder TimeFrameCommand(ulong userId, ulong guildId, string raidString, TimeFrameHours timeFrame, string commandSyntax)
        {
            var embed = new EmbedBuilder();

            int currentTimeFrame = Convert.ToInt32(Math.Floor((DateTime.UtcNow - _bungie.ReleaseDate).TotalHours / (int)timeFrame));

            Raid raid = _raids.GetRaid(guildId, raidString);
            if (raid == null)
            {
                embed.WithTitle($"{commandSyntax} raid completions from {_bungie.ReleaseDate.AddHours(currentTimeFrame * (int)timeFrame):yyyy-MM-dd} to {_bungie.ReleaseDate.AddHours((currentTimeFrame + 1) * (int)timeFrame):yyyy-MM-dd}");
            }
            else
            {
                embed.WithTitle($"{commandSyntax} {raid.DisplayName} completions from {_bungie.ReleaseDate.AddHours(currentTimeFrame * (int)timeFrame):yyyy-MM-dd} to {_bungie.ReleaseDate.AddHours((currentTimeFrame + 1) * (int)timeFrame):yyyy-MM-dd}");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
            }

            IEnumerable<(User user, int completions, int rank)> usersWithCompletionsAndRank = _leaderboards.GetUserCompetionsByTimeframe(_users.GetGuildUsers(guildId), raid, timeFrame, currentTimeFrame);
            IEnumerable<(User user, int completions, int rank)> usersWithMaxCompletionsAndRank = _leaderboards.GetUserCompetionsMaxByTimeframe(_users.GetGuildUsers(guildId), raid, timeFrame);
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
    }
}
