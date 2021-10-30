using ClearsBot.Modules;
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
        readonly ILanguages _languages;
        readonly IRaids _raids;
        public Formatting(ILanguages languages, IRaids raids)
        {
            _languages = languages;
            _raids = raids;
        }
        public string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false)
        {
            string leaderboard = "";
            foreach ((User user, int completions, int rank) user in users.Take(count))
            {
                if (user.user.DiscordID == userDiscordId)
                {
                    leaderboard += string.Format(_languages.GetLanguageText("en", "rank-entry-active"), user.rank, FormatUsername(user.user.Username), user.completions);
                    //leaderboard += $"**{user.rank}) {FormatUsername(user.user.Username)}: {user.completions} completions** \n";
                    continue;
                }

                //leaderboard += $"{user.rank}) {FormatUsername(user.user.Username)}: {user.completions} completions \n";
                leaderboard += string.Format(_languages.GetLanguageText("en", "rank-entry"), user.rank, FormatUsername(user.user.Username), user.completions);
            }

            leaderboard += "\n";

            foreach ((User user, int completions, int rank) user in users.Where(x => x.user.DiscordID == userDiscordId))
            {
                if (users.Take(count).Contains(user)) continue;
                leaderboard += string.Format(_languages.GetLanguageText("en", "rank-entry"), user.rank, FormatUsername(user.user.Username), user.completions);
            }

            if (registerMessage)
            {
                if (users.Where(x => x.user.DiscordID == userDiscordId).Count() <= 0)
                {
                    leaderboard += _languages.GetLanguageText("en", "unregistered-message");
                }
            }

            return leaderboard.Length <= 1024 ? leaderboard : "leaderboard string was too long.";
        }

        public EmbedBuilder GetCompletionsEmbed(User user, IEnumerable<(Raid raid, int completions)> completions)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"Raid completions for {FormatUsername(user.Username)}");
            foreach ((Raid raid, int completions) completion in completions)
            {
                embed.AddField(completion.raid.DisplayName, $"{completion.completions} completions", true);
            }
            return embed;
        }

        public EmbedBuilder GetFastestEmbed(IEnumerable<Completion> completions, string username, string raidName, ulong guildId, Raid raid = null)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"Fastest {raidName} completions for {username}");

            string list = "";
            if (completions.Count() <= 10)
            {
                foreach (Completion completion in completions.Take(completions.Count()))
                {
                    list += $"[{_raids.GetRaids(guildId).FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {string.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
                }
            }
            else
            {
                foreach (Completion completion in completions.Take(10))
                {
                    list += $"[{_raids.GetRaids(guildId).FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {string.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
                }
            }
            embed.Description = list;

            if (raid != null && raid.IconUrl != "")
            {
                embed.WithThumbnailUrl(raid.IconUrl);
                embed.WithColor(raid.Color.R, raid.Color.G, raid.Color.B);
            }
            return embed;
        }
        public string FormatUsername(string username)
        {
            return username.Replace("_", "\\_");
        }
    }
}
