using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearsBot.Modules
{
    public class Formatting : IFormatting
    {
        public string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false)
        {
            string leaderboard = "";
            foreach ((User user, int completions, int rank) user in users.Take(count))
            {
                if (user.user.DiscordID == userDiscordId)
                {
                    leaderboard += $"**{user.rank}) {FormatUserName(user.user.Username)}: {user.completions} completions** \n";
                    continue;
                }

                leaderboard += $"{user.rank}) {FormatUserName(user.user.Username)}: {user.completions} completions \n";
            }

            foreach ((User user, int completions, int rank) user in users.Where(x => x.user.DiscordID == userDiscordId))
            {
                if (users.Take(count).Contains(user)) continue;
                leaderboard += $"\n {user.rank}) {FormatUserName(user.user.Username)}: {user.completions} completions";
            }

            if (registerMessage)
            {
                if (users.Where(x => x.user.DiscordID == userDiscordId).Count() <= 0)
                {
                    leaderboard += "\n You haven't registered.";
                }
            }

            return leaderboard.Length <= 1024 ? leaderboard : "leaderboard string was too long.";
        }

        public string FormatUserName(string username)
        {
            return username.Replace("_", "\\_");
        }
    }
}
