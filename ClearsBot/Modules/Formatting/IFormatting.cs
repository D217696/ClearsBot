using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IFormatting
    {
        public string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false);
        public EmbedBuilder GetCompletionsEmbed(User user, IEnumerable<(Raid raid, int completions)> completions);
        public EmbedBuilder GetFastestEmbed(IEnumerable<Completion> completions, string username, string raidName, ulong guildId);
        public string FormatUsername(string username);
    }
}