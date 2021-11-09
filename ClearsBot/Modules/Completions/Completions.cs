using ClearsBot.Modules;
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

        public IEnumerable<User> FilterCompletionsByRaidCriteria(IEnumerable<User> users, Raid raid = null, ulong guildId = 0)
        {
            if (raid != null)
            {
                var criteria = _raids.GetCriteriaByRaid(raid);
                return users.Select(x => { x.Completions = x.Completions.Values.Where(criteria).ToDictionary(c => c.InstanceID); return x; });
            }

            IEnumerable<Raid> raids = _raids.GetRaids(guildId);
            return users.Select(x => { x.Completions = GetRaidCompletionsListForUser(x, raids).ToDictionary(c => c.InstanceID); return x; });
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

        public IEnumerable<(User user, int completions, int rank)> FilterByTimeFrameMax(IEnumerable<User> users, TimeFrameHours timeFrameHours)
        {
            //DateTime releaseDate = _bungie.ReleaseDate;
            //Func<Completion, string> groupByCriteria = completion => Convert.ToInt32(Math.Floor((completion.Period - releaseDate).TotalHours / (int)timeFrameHours)).ToString();
            //if (timeFrameHours == TimeFrameHours.Month) groupByCriteria = completion => completion.Period.ToString("yyyyMM");

            //var usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.GroupBy(groupByCriteria).Max(completions => completions.Count())));
            //var bv = users.Select(x => (user: x, completions: x.Completions.Values.GroupBy(groupByCriteria)));
            //var cb = bv.Select(x => (x.user, completionCount: x.completions.Max(b => b.Count())));
            //var UsersWithMaxCompletionsOrdered = usersWithMaxCompletionCount.OrderByDescending(x => x.completions).ToList();

            //return UsersWithMaxCompletionsOrdered.Select(x => (x.user, x.completions, rank: UsersWithMaxCompletionsOrdered.IndexOf(x) + 1));

            Func<Completion, string> groupByCriteria = completion => Convert.ToInt32(Math.Floor((completion.Period - _bungie.ReleaseDate).TotalHours / (int)timeFrameHours)).ToString();
            if (timeFrameHours == TimeFrameHours.Month) groupByCriteria = completion => completion.Period.ToString("yyyyMM");

            List<(User user, int completions)> usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.GroupBy(groupByCriteria).Max(completions => completions.Count()))).OrderByDescending(x => x.completions).ToList();
            return usersWithMaxCompletionCount.Select(x => (x.user, x.completions, rank: usersWithMaxCompletionCount.IndexOf(x) + 1));
            Console.WriteLine("");
        }

        public IEnumerable<(User user, int completions, int rank)> FilterByTimeFrameCurrent(IEnumerable<User> users, TimeFrameHours timeFrameHours, int currentTimeFrame)
        {
            List<(User user, int completions)> usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.Where(completion => Convert.ToInt32(Math.Floor((completion.Period - _bungie.ReleaseDate).TotalHours / (int)timeFrameHours)) == currentTimeFrame).Count())).OrderByDescending(x => x.completions).ToList();
            if (timeFrameHours == TimeFrameHours.Month) usersWithMaxCompletionCount = users.Select(x => (user: x, completions: x.Completions.Values.Where(completion => Convert.ToInt32(completion.Period.ToString("yyyyMM")) == currentTimeFrame).Count())).OrderByDescending(x => x.completions).ToList();
            return usersWithMaxCompletionCount.Select(x => (x.user, x.completions, rank: usersWithMaxCompletionCount.IndexOf(x) + 1));
        }

        public IEnumerable<(User user, Completion completion, int rank)> GetFastestRankList(IEnumerable<User> users, Raid raid, ulong guildId)
        {
            // var usersWithFastestCompletions = users.Select(x => (user: x, completion: GetRaidCompletionsListForUser(x, guildId).Where(_raids.GetCriteriaByRaid(raid)).OrderBy(x => x.Time).First())).OrderBy(x => x.completion.Time);
            //List<(User user, Completion completion)> usersWithFastestCompletionList = usersWithFastestCompletions.Where(x => x.completion != null).ToList();

            var userx = users.Select(x => (user: x, completions: GetRaidCompletionsListForUser(x, guildId)));
            var userz = userx.Select(x => (x.user, completions: x.completions.Where(_raids.GetCriteriaByRaid(raid))));
            var usert = userz.Where(x => x.completions.Count() > 0);
            var userb = usert.Select(x => (x.user, competions: x.completions.OrderBy(x => x.Time)));
            var userc = userb.Select(x => (x.user, completion: x.competions.First()));
            var userd = userc.OrderBy(x => x.completion.Time);
            var usere = userd.ToList();
            return usere.Select(x => (x.user, x.completion, rank: usere.IndexOf(x) + 1));
        }

        //public IEnumerable<(User user, Completion completion, int rank)> GetSlowestRankList(IEnumerable<User> users, Raid raid, ulong guildId)
        //{
        //    List<(User user, Completion completion)> usersWithFastestCompletion = users.Select(x => (user: x, completion: GetRaidCompletionsListForUser(x, guildId).Where(_raids.GetCriteriaByRaid(raid)).OrderBy(x => x.Time).Last())).OrderByDescending(x => x.completion.Time).ToList();
        //    return usersWithFastestCompletion.Select(x => (x.user, x.completion, rank: usersWithFastestCompletion.IndexOf(x) + 1));
        //}

        public IEnumerable<(Raid raid, int completions)> GetRaidCompletionsForUser(User user, ulong guildId)
        {
            return _raids.GetRaids(guildId).Select(x => (raid: x, completions: user.Completions.Values.Where(_raids.GetCriteriaByRaid(x)).Count()));
        }

        public IEnumerable<Completion> GetRaidCompletionsListForUser(User user, ulong guildId)
        {
            List<Completion> completions = new List<Completion>();
            foreach (Raid raid in _raids.GetRaids(guildId))
            {
                var criteria = _raids.GetCriteriaByRaid(raid);
                completions.AddRange(user.Completions.Values.Where(criteria));
            }
            return completions;
        }

        public IEnumerable<Completion> GetRaidCompletionsListForUser(User user, IEnumerable<Raid> raids)
        {
            List<Completion> completions = new List<Completion>();
            foreach (Raid raid in raids)
            {
                var criteria = _raids.GetCriteriaByRaid(raid);
                completions.AddRange(user.Completions.Values.Where(criteria));
            }
            return completions;
        }
    }
}
