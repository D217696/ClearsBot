using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearsBot.Modules
{
    public class Leaderboards : ILeaderboards
    {
        readonly IBungie _bungie;
        readonly IRaids _raids;
        public Leaderboards(IBungie bungie, IRaids raids)
        {
            _bungie = bungie;
            _raids = raids;
        }

        public IEnumerable<(User user, int completions, int rank)> GetUserCompetionsMaxByTimeframe(IEnumerable<User> users, Raid raid, TimeFrameHours timeFrameHours)
        {
            List<(User user, int completions)> usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.GroupBy(completion => Convert.ToInt32(Math.Floor((completion.Period - _bungie.ReleaseDate).TotalHours / (int)timeFrameHours))).Max(completions => completions.Where(_raids.GetCriteriaByRaid(raid)).Count()))).OrderByDescending(x => x.completions).ToList();
            if (timeFrameHours == TimeFrameHours.Month) usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.GroupBy(completion => completion.Period.ToString("yyyyMM")).Max(completions => completions.Where(_raids.GetCriteriaByRaid(raid)).Count()))).OrderByDescending(x => x.completions).ToList();
            return usersWithMaxCompletionCount.Select(x => (x.user, x.completions, rank: usersWithMaxCompletionCount.IndexOf(x) + 1));
        }

        public IEnumerable<(User user, int completions, int rank)> GetUserCompetionsByTimeframe(IEnumerable<User> users, Raid raid, TimeFrameHours timeFrameHours, int currentTimeFrame)
        {
            List<(User user, int completions)> usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.Where(completion => Convert.ToInt32(Math.Floor((completion.Period - _bungie.ReleaseDate).TotalHours / (int)timeFrameHours)) == currentTimeFrame).Where(_raids.GetCriteriaByRaid(raid)).Count())).OrderByDescending(x => x.completions).ToList();
            if (timeFrameHours == TimeFrameHours.Month) usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.Where(completion => Convert.ToInt32(completion.Period.ToString("yyyyMM")) == currentTimeFrame).Where(_raids.GetCriteriaByRaid(raid)).Count())).OrderByDescending(x => x.completions).ToList();
            return usersWithMaxCompletionCount.Select(x => (x.user, x.completions, rank: usersWithMaxCompletionCount.IndexOf(x) + 1));
        }

        public string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false)
        {
            string leaderboard = "";
            foreach ((User user, int completions, int rank) user in users.Take(count))
            {
                if (user.user.DiscordID == userDiscordId)
                {
                    leaderboard += $"**{user.rank}) {user.user.Username}: {user.completions} completions** \n";
                    continue;
                }

                leaderboard += $"{user.rank}) {user.user.Username}: {user.completions} completions \n";
            }

            foreach ((User user, int completions, int rank) user in users.Where(x => x.user.DiscordID == userDiscordId))
            {
                if (users.Take(count).Contains(user)) continue;
                leaderboard += $"\n {user.rank}) {user.user.Username}: {user.completions} completions";
            }

            if (registerMessage)
            {
                if (users.Where(x => x.user.DiscordID == userDiscordId).Count() <= 0)
                {
                    leaderboard += "\n You haven't registered.";
                }
            }

            return leaderboard.Length <= 1024 ? leaderboard.Replace("_", "\\_") : "leaderboard string was too long.";
        }
    }
}
