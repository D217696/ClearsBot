using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Utilities : IUtilities
    {
        readonly IRaids _raids;
        public Utilities(IRaids raids)
        {
            _raids = raids;
        }
        public EmbedBuilder GetFastestListForUser(User user, Raid raid, ulong guildID)
        {
            var embed = new EmbedBuilder();
            string list = "";
            List<Completion> completions = new List<Completion>();
            if (raid == null)
            {
                embed.WithTitle($"Fastest raid completions for {user.Username}");
                foreach (Raid raidFromList in _raids.GetRaids(guildID))
                {
                    completions.AddRange(user.Completions.Values.Where(x => x.StartingPhaseIndex <= raidFromList.StartingPhaseIndexToBeFresh && raidFromList.Hashes.Contains(x.RaidHash)));
                }
            }
            else
            {
                embed.WithTitle($"Fastest {raid.DisplayName} completions for {user.Username}");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
                completions = user.Completions.Values.Where(x => x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(x.RaidHash)).ToList();
            }

            if (completions.Count <= 10)
            {
                foreach (Completion completion in completions.OrderBy(x => x.Time).Take(completions.Count))
                {
                    list += $"[{_raids.GetRaids(guildID).FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {String.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
                }
            }
            else
            {
                foreach (Completion completion in completions.OrderBy(x => x.Time).Take(10))
                {
                    list += $"[{_raids.GetRaids(guildID).FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {String.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
                }
            }
            embed.Description = list;
            return embed;
        }
        //[Command("Test")]
        //public async Task Test(string raidString = "")
        //{
        //    Raid raid = Raids.raids[Context.Guild.Id].Where(x => x.Shortcuts.Contains(raidString)).FirstOrDefault();
        //    var users = Users.users[Context.Guild.Id].Select(user => (user, completionCount: user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count())).OrderByDescending(x => x.completionCount);
        //    var embed = new EmbedBuilder();
        //    ulong targetUserId = GetTargetUser(Context);
        //    foreach (var user in users.Take(10))
        //    {
        //        if (user.user.DiscordID == targetUserId)
        //        {
        //            embed.Description += string.Format(Langauges.languages[Guilds.guilds[Context.Guild.Id].Language]["rank-entry-active"], users.ToList().IndexOf(user) + 1, user.user.Username, user.completionCount);
        //            continue;
        //        }

        //        embed.Description += string.Format(Langauges.languages[Guilds.guilds[Context.Guild.Id].Language]["rank-entry"], users.ToList().IndexOf(user) + 1, user.user.Username, user.completionCount);
        //    }

        //    foreach(var user in users.Where(x => x.user.DiscordID == targetUserId))
        //    {
        //        if (users.Take(10).Contains(user)) continue;
        //        embed.Description += string.Format(Langauges.languages[Guilds.guilds[Context.Guild.Id].Language]["rank-entry-active"], users.ToList().IndexOf(user) + 1, user.user.Username, user.completionCount);
        //    }

        //    await ReplyAsync(embed: embed.Build());
        //    //if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminUser)
        //    //{
        //    //    await ReplyAsync("No permission");
        //    //    return;
        //    //}

        //    //foreach (User user in Users.users[Context.Guild.Id])
        //    //{
        //    //    user.DateLastPlayed = new DateTime(2017, 01, 01, 0, 0, 0);
        //    //    user.Completions = new Dictionary<long, Completion>();
        //    //    user.Characters = new List<Character>();
        //    //}

        //    //await ReplyAsync("Set every datelastplayed to 2017/01/01");
        //}

        public EmbedBuilder GetCompletionsForUser(User user, ulong guildId)
        {
            var embed = new EmbedBuilder() { Title = $"Raid completions for {user.Username}" };
            foreach (Raid raid in _raids.GetRaids(guildId))
            {
                embed.AddField(raid.DisplayName, user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count() + " completions", true);
            }

            return embed;
        }

        public int GetCompletionCountForUser(User user, ulong guildId)
        {
            int completions = 0;
            foreach (Raid raid in _raids.GetRaids(guildId))
            {
                completions += user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count();
            }
            return completions;
        }

        public Func<Completion, bool> GetCriteriaForRaid(Raid raid)
        {
            if (raid == null) return x => x.StartingPhaseIndex <= 0;
            return x => raid.Hashes.Contains(x.RaidHash) && x.Time > raid.CompletionTime && x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh;
        }

        public ComponentBuilder GetButtonsForUser(List<User> users, ulong guildId, string command, User firstUser = null, string extra = "")
        {
            var componentBuilder = new ComponentBuilder();

            int buttons = 0;
            int buttonRow = 0;
            foreach (User user in users)
            {
                if (user == firstUser) continue;
                if (extra == "")
                {
                    componentBuilder.WithButton(new ButtonBuilder().WithLabel(user.Username).WithCustomId($"{command}_{guildId}_{user.MembershipId}").WithStyle(GetButtonStyleForPlatform(user.MembershipType)), 0);
                }
                else
                {
                    componentBuilder.WithButton(new ButtonBuilder().WithLabel(user.Username).WithCustomId($"{command}_{guildId}_{user.MembershipId}_{extra}").WithStyle(GetButtonStyleForPlatform(user.MembershipType)), 0);
                }
                buttons++;
                if (buttons % 5 == 0) buttonRow++;
            }

            return componentBuilder;
        }

        public DateTime GetWeeklyResetTime(DateTime startDate)
        {
            DateTime lastReset = startDate.AddDays((((int)DayOfWeek.Tuesday) - (((int)startDate.ToUniversalTime().DayOfWeek) + 7)) % 7);

            if (startDate.ToUniversalTime().DayOfWeek == DayOfWeek.Tuesday && startDate.ToUniversalTime().TimeOfDay < new TimeSpan(17, 0, 0))
            {
                lastReset = lastReset.AddDays(-7);
            }

            return new DateTime(lastReset.Year, lastReset.Month, lastReset.Day, 17, 0, 0);
        }

        public Func<Completion, bool> RaidCriteria(Raid raid) => completion => completion.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(completion.RaidHash) && completion.Time > raid.CompletionTime;
        public Func<Completion, bool> StartDateCriteria(DateTime? startDate) => completion => completion.Period > startDate;
        public Func<Completion, bool> EndDateCriteria(DateTime? endDate) => completion => completion.Period < endDate;
        public Func<Completion, bool> DefaultCriteria() => completion => completion.StartingPhaseIndex <= 1;
        public Func<User, int> Criteria(Raid raid = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            Func<Completion, bool> InnerCriteria() =>
            (raid, startDate, endDate) switch
            {
                ({ }, { }, { }) => RaidCriteria(raid) + StartDateCriteria(startDate) + EndDateCriteria(endDate),
                ({ }, { }, null) => RaidCriteria(raid) + StartDateCriteria(startDate),
                ({ }, null, { }) => RaidCriteria(raid) + EndDateCriteria(endDate),
                ({ }, null, null) => RaidCriteria(raid),
                (null, { }, { }) => StartDateCriteria(startDate) + EndDateCriteria(endDate),
                (null, { }, null) => StartDateCriteria(startDate),
                (null, null, { }) => EndDateCriteria(endDate),
                (null, null, null) => DefaultCriteria()
            };

            return x => x.Completions.Values.Where(InnerCriteria()).Count();
        }

        public int GetCompletionCountForUser(User user, Raid raid = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<Completion> completions = user.Completions.Values;
            if (raid == null && startDate == null && endDate == null) return completions.Where(DefaultCriteria()).Count();
            if (raid != null)
            {
                completions = completions.Where(RaidCriteria(raid));
            }

            if (startDate != null)
            {
                completions = completions.Where(StartDateCriteria(startDate));
            }

            if (endDate != null)
            {
                completions = completions.Where(EndDateCriteria(endDate));

            }

            return completions.Count();
        }

        public ButtonStyle GetButtonStyleForPlatform(int membershipType)
        {
            return membershipType switch
            {
                1 => ButtonStyle.Success,
                2 => ButtonStyle.Primary,
                3 => ButtonStyle.Danger,
                _ => ButtonStyle.Secondary
            };
        }

        public string GetErrorCodeForUserSearch(RequestData requestData)
        {
            return requestData.Code switch
            {
                2 => "Please enter an ID or register.",
                3 => "Please enter a valid membership type.",
                4 => "User not found",
                5 => "Please enter a valid Steam Id.",
                6 => "Please enter a valid BungieID",
                9 => "Profile not found.",
                _ => requestData.DisplayName
            };
        }
    }
}
