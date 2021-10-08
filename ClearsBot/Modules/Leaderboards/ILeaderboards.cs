using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface ILeaderboards
    {
        public IEnumerable<(User user, int completions, int rank)> GetUserCompetionsMaxByTimeframe(IEnumerable<User> users, Raid raid, TimeFrameHours timeFrameHours);
        public IEnumerable<(User user, int completions, int rank)> GetUserCompetionsByTimeframe(IEnumerable<User> users, Raid raid, TimeFrameHours timeFrameHours, int currentTimeFrame);
        public string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false);
    }
}