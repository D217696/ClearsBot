using ClearsBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearsBot.Modules
{
    public class Completions
    {
        private readonly Users _users;
        private readonly IRaids _raids;
        private readonly IBungie _bungie;
        public Completions(Users users, IRaids raids, IBungie bungie)
        {
            _users = users;
            _raids = raids;
            _bungie = bungie;
        }

        public IEnumerable<(User user, int completions, int rank)> GetCompletionsForUsers(List<User> users, DateTime startDate, DateTime endDate, IEnumerable<Raid> raids)
        {
            if (raids.Count() <= 0) return null;

            if (raids.Count() == 1)
            {
                List<(User user, int completions)> usersList = users.Select(x => (user: x, completions: x.Completions.Values.Where(_raids.GetCriteriaByRaid(raids.FirstOrDefault())).Where(x => x.Period > startDate && x.Period < endDate).Count())).ToList().OrderByDescending(x => x.completions).ToList();
                return usersList.Select(x => (x.user, x.completions, rank: usersList.IndexOf(x) + 1));
            }

            List<(User user, int completions)> userList = new List<(User, int)>();
            foreach (User user in users)
            {
                int completions = 0;
                foreach (Raid raid in raids)
                {
                    completions += user.Completions.Values.Where(_raids.GetCriteriaByRaid(raid)).Where(x => x.Period > startDate && x.Period < endDate).Count();
                }
                userList.Add((user, completions));
            }

            return userList.OrderByDescending(x => x.completions).Select(x => (x.user, x.completions, rank: userList.OrderByDescending(x => x.completions).ToList().IndexOf(x) + 1));
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

        public IEnumerable<(Raid raid, int completions)> GetRaidCompletionsForUser(User user, ulong guildId)
        {
            return _raids.GetRaids(guildId).Select(x => (raid: x, completions: user.Completions.Values.Where(_raids.GetCriteriaByRaid(x)).Count()));
        }
    }
}
