using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        Bungie _bungie;
        public Misc(Bungie bungie)
        {
            _bungie = bungie;
        }

        [Command("Register", RunMode = RunMode.Async)]
        public async Task Register(string membershipId = "", string membershipType = "")
        {
            await RegisterUser(Context.Channel, Context.Guild.Id, Context.User.Id, Context.User.Username, membershipId, membershipType);
        }

        [Command("Completions")]
        public async Task Completions()
        {
            List<User> users = Users.GetUsers(Context);
            if (users.Count == 0)
            {
                await Context.Channel.SendMessageAsync("You have not registered.");
                return;
            }

            await ReplyAsync(embed: GetCompletionsForUser(users.FirstOrDefault(), Context.Guild.Id).Build(), component: GetButtonsForUser(users, Context.Guild.Id, "completions", users.FirstOrDefault()).Build());
        }

        [Command("Rank")]
        public async Task Rank(string raidString = "", string countString = "")
        {
            bool ParseSucceeded = int.TryParse(countString, out int count);
            if (!ParseSucceeded) count = 10;

            ulong targetUserId = GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            Raid raid = Raids.raids[Context.Guild.Id].Where(x => x.Shortcuts.Contains(raidString)).FirstOrDefault();
            var embed = new EmbedBuilder();
            if (raid == null)
            {
                embed.WithTitle($"Top {count} for all raid completions");
            }
            else
            {
                embed.WithTitle($"Top {count} for {raid.DisplayName} completions");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
            }

            int rank = 1;
            var x = Users.users;
            List<User> users = GetListOfUsersSorted(raid, Context.Guild.Id);
            foreach (User user in users.Take(count))
            {
                if (user.DiscordID == targetUserId)
                {
                    embed.Description += $"**{rank}) {user.Username}: {user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count() } completions**\n"; //string.Format(Langauges.languages[Guilds.guilds[Context.Guild.Id].Language]["rank-entry-active"], rank.ToString(), user.Username, user.Completions.Values.Where(criteria).Count());
                }
                else
                {
                    embed.Description += $"{rank}) {user.Username}: {user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count() } completions\n"; //string.Format(Langauges.languages[Guilds.guilds[Context.Guild.Id].Language]["rank-entry"], rank.ToString(), user.Username, user.Completions.Values.Where(criteria).Count());
                }

                rank++;
            }

            List<User> usersFromTargetUser = Users.GetUsers(Context);
            foreach(User user in usersFromTargetUser.OrderBy(x => users.IndexOf(x)))
            {
                if (users.Take(count).Contains(user)) continue;
                int rankuser = users.IndexOf(user) + 1;
                embed.Description += $"\n{rankuser}) {user.Username}: {user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count()} completions";
            }
            if (usersFromTargetUser.Count == 0)
            {
                embed.Description += "\n You haven't registered";
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Yearly")]
        public async Task Yearly(string raidString = "", string countString = "", string date = "")
        {
            ulong targetUserId = GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: CreateLeaderboardMessage(-358, 365, raidString, Context.Guild.Id, targetUserId, 8760, "this year", countString).Build());
        }

        [Command("Monthly")]
        public async Task Monthly(string raidString = "", string countString = "", string date = "")
        {
            ulong targetUserId = GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: CreateLeaderboardMessage(-21, 28, raidString, Context.Guild.Id, targetUserId, 672, "this month", countString).Build());
        }


        [Command("Weekly")]
        public async Task Weekly(string raidString = "", string countString = "", string date = "")
        {
            ulong targetUserId = GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: CreateLeaderboardMessage(0, 7, raidString, Context.Guild.Id, targetUserId, 168, "this week", countString).Build()); 
        }

        [Command("Daily")]
        public async Task Daily(string raidString = "", string countString = "", string date = "")
        {
            DateTime startDate = DateTime.UtcNow.TimeOfDay < new TimeSpan(17, 0, 0) ? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 17, 0, 0).AddDays(-1) : new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 17, 0, 0);

            ulong targetUserId = GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: CreateLeaderboardMessage(0, 1, raidString, Context.Guild.Id, targetUserId, 24, "today", countString, startDate).Build());
        }

        [Command("Update")]
        public async Task Update()
        {
            GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(Context.Guild.Id, Context.User.Id);
            if (getCompletionsResponse.Code != 1)
            {
                await Context.Channel.SendMessageAsync(getCompletionsResponse.ErrorMessage);
                return;
            }

            await ReplyAsync($"{Users.users[Context.Guild.Id].FirstOrDefault(x => x.DiscordID == Context.User.Id).Completions.Count()} completions");
        }

        [Command("Raids")]
        public async Task ShowRaids()
        {
            var embed = new EmbedBuilder();
            foreach (Raid raid in Raids.raids[Context.Guild.Id])
            {
                string value = "**Shortcuts**\n";
                foreach (string shortcut in raid.Shortcuts)
                {
                    value += shortcut + "\n";
                }
                embed.AddField(raid.DisplayName, value, true);
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("AddShortcut")]
        public async Task AddShortcut(string raidString = "", string shortcut = "")
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.ModRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            if (Context.User.Id != Config.bot.owner)
            {
                await Context.Channel.SendMessageAsync("No permissions");
                return;
            }

            Raid raid = Raids.raids[Context.Guild.Id].FirstOrDefault(x => x.Shortcuts.Contains(raidString) || x.DisplayName.ToLower().Contains(raidString));
            if (raid != null)
            {
                if (!raid.Shortcuts.Contains(shortcut))
                {
                    raid.Shortcuts.Add(shortcut);
                    Raids.SaveRaids();
                    await Context.Channel.SendMessageAsync($"Added shortcut: {shortcut} to {raid.DisplayName}");
                    return;
                }

                await Context.Channel.SendMessageAsync($"{raid.DisplayName} already has that shortcut!");
                return;
            }

            await ReplyAsync("Raid not found!");
        }

        [Command("SetTime")]
        public async Task SetTime(string shortcut = "", string time = "")
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            Raid raid = Raids.raids[Context.Guild.Id].Where(x => x.Shortcuts.Contains(shortcut.ToLower()) || x.DisplayName.ToLower().Contains(shortcut.ToLower())).FirstOrDefault();
            if (raid == null)
            {
                await Context.Channel.SendMessageAsync("Raid not found.");
                return;
            }

            if (time.Length == 5 && time.Contains(":"))
            {
                time = "00:" + time;
            }

            bool parseSucceeded = TimeSpan.TryParse(time, out TimeSpan completionTime);
            if (parseSucceeded)
            {
                raid.CompletionTime = completionTime;
                Raids.SaveRaids();
                await Context.Channel.SendMessageAsync($"{raid.DisplayName} completion time set to: {raid.CompletionTime}");
                return;
            }

            await ReplyAsync("Could not convert time to TimeSpan format: hh:mm:ss");
        }

        [Command("Profiles")]
        public async Task Profiles([Remainder] string remainder = "")
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Profiles linked to your account");
            List<User> users = Users.GetUsers(Context);
            foreach(User user in users)
            {
                string platform = Bungie.GetPlatformString(user.MembershipType);
                embed.AddField(user.Username, $"{platform} \n Characters: {user.Characters.Count()} \n Saved pgcrs: {user.Completions.Count()} \n Date last played: {user.DateLastPlayed} \n SteamID: {user.SteamID}", true);
            }

            await  ReplyAsync(embed: embed.Build());
        }

        [Command("GenerateRoles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task GenerateRoles()
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminUser)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            var embed = new EmbedBuilder();
            foreach (Raid raid in Raids.raids[Context.Guild.Id])
            {
                if (raid.FirstRole != 0) continue;
                var firstRole = Context.Guild.Roles.Where(x => x.Name == "#1 " + raid.DisplayName).FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#1 " + raid.DisplayName, null, new Color(raid.Color.R, raid.Color.G, raid.Color.B), false, false, null);
                var secondRole = Context.Guild.Roles.Where(x => x.Name == "#2 " + raid.DisplayName).FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#2 " + raid.DisplayName, null, new Color(raid.Color.R, raid.Color.G, raid.Color.B), false, false, null);
                var thirdRole = Context.Guild.Roles.Where(x => x.Name == "#3 " + raid.DisplayName).FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#3 " + raid.DisplayName, null, new Color(raid.Color.R, raid.Color.G, raid.Color.B), false, false, null);

                raid.FirstRole = firstRole.Id;
                raid.SecondRole = secondRole.Id;
                raid.ThirdRole = thirdRole.Id;

                embed.AddField(raid.DisplayName, $"{firstRole.Mention} \n {secondRole.Mention} \n {thirdRole.Mention}");
            }

            var firstRoleTotal = Context.Guild.Roles.Where(x => x.Name == "#1 total").FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#1 total", null, new Color(255, 255, 255), false, false, null);
            var secondRoleTotal = Context.Guild.Roles.Where(x => x.Name == "#2 total").FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#2 total", null, new Color(255, 255, 255), false, false, null);
            var thirdRoleTotal = Context.Guild.Roles.Where(x => x.Name == "#3 total").FirstOrDefault() ?? (IRole)await Context.Guild.CreateRoleAsync("#3 total", null, new Color(255, 255, 255), false, false, null);

            Guilds.guilds[Context.Guild.Id].FirstRole = firstRoleTotal.Id;
            Guilds.guilds[Context.Guild.Id].SecondRole = secondRoleTotal.Id;
            Guilds.guilds[Context.Guild.Id].ThirdRole = thirdRoleTotal.Id;

            embed.AddField("Total", $"{firstRoleTotal.Mention} \n {secondRoleTotal.Mention} \n {thirdRoleTotal.Mention}");

            Guilds.SaveGuilds();
            Raids.SaveRaids();

            await ReplyAsync(embed: embed.Build());
        }

        [Command("SetPrefix")]
        public async Task SetPrefix(string prefix)
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            Guilds.guilds[Context.Guild.Id].Prefix = prefix;
            Guilds.SaveGuilds();
            await ReplyAsync($"Prefix has been set to: {prefix}");
        }

        [Command("Help")]
        public async Task Help([Remainder] string remainder = "")
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor("Help");
            embed.AddField("$register", "Registers you to the bot. \n usage: $register (joincode | username | membership id) (membership type (optional))");
            embed.AddField("$completions", "Shows your raid completions per raid using the bot's criteria. \n usage: $completions");
            embed.AddField("$rank", "Shows a list of completions, ordered by count. \n usage: $rank (raid) (count)");
            embed.AddField("$monthly", "Shows a list of completions this month, ordered by count. \n usage: $monthly (raid) (count)");
            embed.AddField("$weekly", "Shows a list of completions this week, ordered by count. \n usage: $weekly (raid) (count)");
            embed.AddField("$daily", "Shows a list of completions today, ordered by count. \n usage: $daily (raid) (count)");
            embed.AddField("$update", "Updates your raid completions. \n usage: $update");
            embed.AddField("$raids", "Shows a list of the raids, along with the shortcuts that can be used for this raid. \n usage: $raids");
            embed.AddField("$help", "A reference to help could not be added. Adding this command as a reference would cause a circular dependency.");

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Unregister")]
        public async Task Unregister()
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("Profiles linked to account");
            List<User> users = Users.GetUsers(Context);
            foreach (User user in users)
            {
                string platform = Bungie.GetPlatformString(user.MembershipType);
                embed.AddField(user.Username, $"{platform} \n Characters: {user.Characters.Count()} \n Saved pgcrs: {user.Completions.Count()} \n Date last played: {user.DateLastPlayed} \n SteamID: {user.SteamID}", true);
            }

            await ReplyAsync(embed: embed.Build(), component: GetButtonsForUser(users, Context.Guild.Id, "unregister").Build());
        }

        [Command("reglist")]
        public async Task GetUsers()
        {
            var embed = new EmbedBuilder
            {
                Title = $"Users page (1/{Math.Ceiling((decimal)Users.users[Context.Guild.Id].Count() / 10)} total users: {Users.users[Context.Guild.Id].Count()})"
            };

            foreach (User user in GetUsersByPage(Context.Guild.Id))
            {
                embed.Description += $"<@!{user.DiscordID}>: {user.Username} \n";
            }

            if (Math.Ceiling((double)Users.users[Context.Guild.Id].Count() / 10) > 1)
            {
                var componentBuilder = new ComponentBuilder();

                componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Danger).WithLabel("next").WithCustomId($"reglist_{Context.Guild.Id}_2_{Context.User.Id}"));
                await ReplyAsync(embed: embed.Build(), component: componentBuilder.Build());
                return;
            }

            await ReplyAsync(embed: embed.Build());

            //await ReplyAsync($"Number of bungie profiles being tracked: **{Users.users[Context.Guild.Id].Count()}** \nNumber of different discord users being tracked: **{Users.users[Context.Guild.Id].GroupBy(x => x.DiscordID).Count()}**");
        }

        [Command("CreateMilestone")]
        public async Task CreateMilestone(string completions, string role)
        {
            if(Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminRole)
            {
                await ReplyAsync("No permission");
                return;
            }

            bool parseSucceed = int.TryParse(completions, out int completionCount);
            if (!parseSucceed)
            {
                await Context.Channel.SendMessageAsync("Please enter a valid number");
                return;
            }

            SocketRole socketRole = Context.Guild.Roles.Where(x => x.Mention == role).FirstOrDefault();
            if (socketRole == null)
            {
                await Context.Channel.SendMessageAsync("Role not found");
                return;
            }

            Guilds.guilds[Context.Guild.Id].Milestones.Add(new Milestone()
            {
                Completions = completionCount,
                Role = socketRole.Id
            });

            Guilds.SaveGuilds();
            await ReplyAsync($"Milestone created for {completionCount} completions, {socketRole.Mention}");
        }

        [Command("Admin")]
        public async Task AddAdmin(string role)
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminUser)
            {
                await ReplyAsync("No permission");
                return;
            }

            SocketRole socketRole = Context.Guild.Roles.Where(x => x.Mention == role).FirstOrDefault();
            if(socketRole == null)
            {
                await ReplyAsync("Role not found.");
                return;
            }

            Guilds.guilds[Context.Guild.Id].AdminRole = socketRole.Id;
            Guilds.SaveGuilds();
            await ReplyAsync($"{socketRole.Mention} set as Admin role");
        }

        [Command("Mod")]
        public async Task AddMod(string role)
        {
            if (Guilds.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < Permissions.PermissionLevels.AdminRole)
            {
                await ReplyAsync("No permission");
                return;
            }

            SocketRole socketRole = Context.Guild.Roles.Where(x => x.Mention == role).FirstOrDefault();
            if (socketRole == null)
            {
                await ReplyAsync("Role not found.");
                return;
            }

            Guilds.guilds[Context.Guild.Id].ModRoles.Add(socketRole.Id);
            Guilds.SaveGuilds();
            await ReplyAsync($"{socketRole.Mention} set as mod role");
        }

        [Command("Verify", RunMode = RunMode.Async)]
        public async Task Verify()
        {
            List<User> users = Users.GetUsers(Context);
            var message = await ReplyAsync("Verifying your activities..");
            int completions = 0;
            int newCompletions = 0;
            foreach(User user in users)
            {
                completions += user.Completions.Count;
                user.DateLastPlayed = new DateTime(2017, 01, 01, 0, 0, 0);
                foreach(Character character in user.Characters)
                {
                    character.Deleted = false;
                    character.Handled = false;
                }
                await _bungie.GetCompletionsForUserAsync(Context.Guild.Id, user.DiscordID, user.MembershipId);
                newCompletions += user.Completions.Count;
            }

            await message.ModifyAsync(x => x.Content = $"Found {newCompletions - completions} new activities");
        }

        [Command("Fastest")]
        public async Task Fastest(string raidString = "")
        {
            Raid raid = null;
            if(raidString != "")
            {
                raid = Raids.raids[Context.Guild.Id].FirstOrDefault(x => x.Shortcuts.Contains(raidString) || x.DisplayName.ToLower().Contains(raidString));
            }

            ulong targetUserId = GetTargetUser(Context);

            await ReplyAsync(embed: GetFastestListForUser(Users.users[Context.Guild.Id].FirstOrDefault(x => x.DiscordID == targetUserId), raid, Context.Guild.Id).Build(), component: GetButtonsForUser(Users.users[Context.Guild.Id].Where(x => x.DiscordID == targetUserId).ToList(), Context.Guild.Id, "fastest", Users.users[Context.Guild.Id].FirstOrDefault(x => x.DiscordID == targetUserId), raidString).Build());
        }

        public static EmbedBuilder GetFastestListForUser(User user, Raid raid, ulong guildID)
        {
            var embed = new EmbedBuilder();
            string list = "";
            List<Completion> completions = new List<Completion>();
            if (raid == null)
            {
                embed.WithTitle($"Fastest raid completions for {user.Username}");
                foreach (Raid raidFromList in Raids.raids[guildID])
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
                    list += $"[{Raids.raids[guildID].FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {String.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
                }
            }
            else
            {
                foreach (Completion completion in completions.OrderBy(x => x.Time).Take(10))
                {
                    list += $"[{Raids.raids[guildID].FirstOrDefault(x => x.Hashes.Contains(completion.RaidHash)).DisplayName}: {String.Format("{0:hh\\:mm\\:ss}", completion.Time)}](https://raid.report/pgcr/{completion.InstanceID}) \n";
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

        [Command("lev")]
        [Alias("levi")]
        public async Task Levi()
        {
            await Rank("levi");
        }

        [Command("eow")]
        [Alias("eater")]
        public async Task Eow()
        {
            await Rank("eater");
        }

        [Command("sos")]
        [Alias("spire")]
        public async Task Sos()
        {
            await Rank("spire");
        }

        [Command("lw")]
        [Alias("wish")]
        public async Task Lw()
        {
            await Rank("lw");
        }

        [Command("sotp")]
        [Alias("scourge")]
        public async Task Sotp()
        {
            await Rank("sotp");
        }

        [Command("cos")]
        [Alias("crown")]
        public async Task Cos()
        {
            await Rank("cos");
        }

        [Command("gos")]
        [Alias("garden")]
        public async Task Garden()
        {
            await Rank("garden");
        }

        [Command("dsc")]
        [Alias("deep")]
        public async Task Dsc()
        {
            await Rank("dsc");
        }

        [Command("vog")]
        [Alias("vault")]
        public async Task Vault()
        {
            await Rank("vog");
        }


        public static IEnumerable<User> GetUsersByPage(ulong guildId, int index = 1)
        {
            //List<GroupedUser> users = (List<GroupedUser>) Users.users[guildId].GroupBy(x => x.DiscordID).Select(x => new GroupedUser()
            //{
            //    DiscordID = x.Select(y => y.DiscordID).FirstOrDefault(),
            //    Users = x.Where(y => y.DiscordID == x.FirstOrDefault().DiscordID).ToList()
            //}).OrderBy(x => x.Users.Max(y => y.DateRegistered));

            return Users.users[guildId].Where(x => Users.users[guildId].IndexOf(x) <= (index * 10) - 1 && Users.users[guildId].IndexOf(x) > (index * 10) - 11);
        }

        public static EmbedBuilder GetCompletionsForUser(User user, ulong guildId)
        {
            var embed = new EmbedBuilder() { Title = $"Raid completions for {user.Username}" };
            foreach (Raid raid in Raids.raids[guildId])
            {
                embed.AddField(raid.DisplayName, user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count() + " completions", true);
            }

            return embed;
        }

        public static int GetCompletionCountForUser(User user, ulong guildId)
        {
            int completions = 0;
            foreach (Raid raid in Raids.raids[guildId])
            {
                completions += user.Completions.Values.Where(GetCriteriaForRaid(raid)).Count();
            }
            return completions;
        }

        public static Func<Completion, bool> GetCriteriaForRaid(Raid raid)
        {
            if (raid == null) return x => x.StartingPhaseIndex <= 0;
            return x => raid.Hashes.Contains(x.RaidHash) && x.Time > raid.CompletionTime && x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh;
        }

        public static ComponentBuilder GetButtonsForUser(List<User> users, ulong guildId, string command, User firstUser = null, string extra = "")
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

        public static DateTime GetWeeklyResetTime(DateTime startDate)
        {
            DateTime lastReset = startDate.AddDays((((int)DayOfWeek.Tuesday) - (((int)startDate.ToUniversalTime().DayOfWeek) + 7)) % 7);

            if (startDate.ToUniversalTime().DayOfWeek == DayOfWeek.Tuesday && startDate.ToUniversalTime().TimeOfDay < new TimeSpan(17, 0, 0))
            {
                lastReset = lastReset.AddDays(-7);
            }

            return new DateTime(lastReset.Year, lastReset.Month, lastReset.Day, 17, 0, 0);
        }

        public static List<User> GetListOfUsersSorted(Raid raid, ulong guildId, DateTime? startDate = null, DateTime? endDate = null)
        {
            Func<User, int> Criteria() =>
                (raid, startDate) switch
                {
                    ({ }, { }) => x => x.Completions.Where(y => y.Value.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(y.Value.RaidHash) && y.Value.Period > startDate && y.Value.Period < endDate && y.Value.Time > raid.CompletionTime).Count(),
                    (null, { }) => x => x.Completions.Where(y => y.Value.StartingPhaseIndex <= 0 && y.Value.Period > startDate && y.Value.Period < endDate).Count(),
                    ({ }, null) => x => x.Completions.Where(y => y.Value.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(y.Value.RaidHash) && y.Value.Time > raid.CompletionTime).Count(),
                    (null, null) => x => x.Completions.Where(y => y.Value.StartingPhaseIndex <= 0).Count()
                };
            return Users.users[guildId].OrderByDescending(Criteria()).ToList();
        }

        public async Task RegisterUser(ISocketMessageChannel channel, ulong guildId, ulong userId, string discordUsername, string membershipId = "", string membershipType = "", RestFollowupMessage restFollowupMessage = null)
        {
            Users.busy = true;
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
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription(GetErrorCodeForUserSearch(requestData)).Build());
                Users.busy = false;
                return;
            }
            else if(requestData.Code == 8)
            {
                int buttonCount = 0;
                int buttonRow = 0;
                var componentBuilder = new ComponentBuilder();
                foreach (UserInfoCard userInfoCard in requestData.profiles)
                {
                    ButtonStyle buttonStyle = GetButtonStyleForPlatform(userInfoCard.MembershipType);

                    componentBuilder.WithButton(new ButtonBuilder().WithLabel(userInfoCard.DisplayName).WithCustomId($"register_{userInfoCard.MembershipId}_{userInfoCard.MembershipType}").WithStyle(buttonStyle), buttonRow);
                    buttonCount++;
                    if (buttonCount % 5 == 0) buttonRow++;
                }
                await restUserMessage.ModifyAsync(x => x.Components = componentBuilder.Build());
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("Multiple profiles found, select correct one below").Build());
                Users.busy = false;
                return;
            }

            UserResponse userResponse = await Users.CreateAndAddUser(guildId, userId, requestData);
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
                    await restUserMessage.ModifyAsync(x => x.Embed =  embed.WithDescription("There's already a discord account linked to this profile.").Build());
                    return;
            }

            userResponse.User.DateLastPlayed = new DateTime(2017, 08, 06, 0, 0, 0);
            GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(guildId, userId, requestData.MembershipId);
            switch (getCompletionsResponse.Code)
            {
                case 1:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Activities found: {Users.users[guildId].Where(x => x.DiscordID == userId && x.MembershipId == requestData.MembershipId).FirstOrDefault().Completions.Count} raid completions").Build());
                    break;
                default:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Something went wrong: {getCompletionsResponse.ErrorMessage}").Build());
                    break;
            }

            Users.busy = false;
        }

        public EmbedBuilder CreateLeaderboardMessageNew(DateTime startDate, string raidString, ulong guildId, ulong userId, int hours, string countString, string timespanText)
        {
            int currentPeriodIndex = Convert.ToInt32(Math.Floor((startDate - Bungie.ReleaseDate).TotalHours / hours));
            
            var completions = Users.users[guildId].FirstOrDefault(x => x.DiscordID == userId).Completions.Values.Where(x => Convert.ToInt32(Math.Floor((x.Period - Bungie.ReleaseDate).TotalHours / hours)) == currentPeriodIndex );
            bool ParseSucceeded = int.TryParse(countString, out int count);
            if (!ParseSucceeded) count = 10;


            Raid raid = Raids.raids[guildId].Where(x => x.Shortcuts.Contains(raidString)).FirstOrDefault();

            var embed = new EmbedBuilder();
            Func<Completion, bool> criteria = null;
            if (raid == null)
            {
                criteria = x => x.StartingPhaseIndex <= 0 && Convert.ToInt32(Math.Floor((x.Period - Bungie.ReleaseDate).TotalHours / hours)) == currentPeriodIndex;
                embed.WithTitle($"Top {count} for all raid completions {timespanText} ()");
            }
            else
            {
                criteria = x => raid.Hashes.Contains(x.RaidHash) && x.Time > raid.CompletionTime && x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && Convert.ToInt32(Math.Floor((x.Period - Bungie.ReleaseDate).TotalHours / hours)) == currentPeriodIndex;
                embed.WithTitle($"Top {count} for {raid.DisplayName} completions {timespanText} ()");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));
            }

            var orderdUsersX = Users.users[guildId].Select(x => (user: x, completions: x.Completions.Values.Where(criteria).Count())).OrderByDescending(x => x.completions).ToList();
            var orderdUsersWithRankX = orderdUsersX.Select(x => (x.user, x.completions, rank: orderdUsersX.IndexOf(x) + 1));
            embed.AddField($"Completions {timespanText}", CreateLeaderboardString(orderdUsersWithRankX, userId, count, true), true);

            //var users2 = Users.users[guildId].Where(x => x.Completions.Count > 0);
            //Func<KeyValuePair<long, Completion>, int> getWeekNumberFromDate = y => Convert.ToInt32(Math.Floor((y.Value.Period - Bungie.ReleaseDate).TotalHours / hours));
            //Func<KeyValuePair<long, Completion>, bool> getCompletionNoRaid = z => z.Value.StartingPhaseIndex <= 0;
            //var usersAndMaxPeriodCounts = users2.Select(user => (user, completions: user.Completions.GroupBy(getWeekNumberFromDate).Max(g => g.Where(getCompletionNoRaid).Count())));
            //if (raid != null)
            //{
            //    Func<KeyValuePair<long, Completion>, bool> getCompletionRaid = z => z.Value.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(z.Value.RaidHash) && z.Value.Time > raid.CompletionTime;
            //    usersAndMaxPeriodCounts = users2.Select(user => (user, completions: user.Completions.GroupBy(getWeekNumberFromDate).Max(g => g.Where(getCompletionRaid).Count())));
            //}

            //var orderedUsers = usersAndMaxPeriodCounts.OrderByDescending(u => u.completions).ToList();
            //var orderedUsersWithRank = orderedUsers.Select(x => (x.user, x.completions, rank: orderedUsers.IndexOf(x) + 1));

            //embed.AddField("-", "-", true);
            //embed.AddField("Completion leaderboard", CreateLeaderboardString(orderedUsersWithRank, userId, count), true);
            return embed;
        }
        public EmbedBuilder CreateLeaderboardMessage(int daysToAddToLastReset, int offsetDaysToEndDate, string raidString, ulong guildId, ulong userId, int hoursToDivideBy, string timespanText, string countString, DateTime? startDate = null)
        {
            bool ParseSucceeded = int.TryParse(countString, out int count);
            if (!ParseSucceeded) count = 10;

            DateTime lastResetWithTime = GetWeeklyResetTime(DateTime.UtcNow).AddDays(daysToAddToLastReset);
            if (startDate != null)
            {
                lastResetWithTime = (DateTime)startDate;
            }
            DateTime nextResetWithTime = lastResetWithTime.AddDays(offsetDaysToEndDate);

            Raid raid = Raids.raids[guildId].Where(x => x.Shortcuts.Contains(raidString)).FirstOrDefault();

            var embed = new EmbedBuilder();
            Func<Completion, bool> criteria = null;
            IEnumerable<(User, int, int)> orderdUsersWithRankX = null;
            if (raid == null)
            {
                criteria = x => x.StartingPhaseIndex <= 0 && x.Period > lastResetWithTime && x.Period < nextResetWithTime;
                embed.WithTitle($"Top {count} for all raid completions {timespanText} ({lastResetWithTime.ToShortDateString()} - {nextResetWithTime.ToShortDateString()})");
                orderdUsersWithRankX = GetListOfUsersWithCompletions(guildId, lastResetWithTime, nextResetWithTime);
            }
            else
            {
                criteria = x => raid.Hashes.Contains(x.RaidHash) && x.Time > raid.CompletionTime && x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && x.Period > lastResetWithTime && x.Period < nextResetWithTime;
                embed.WithTitle($"Top {count} for {raid.DisplayName} completions {timespanText} ({lastResetWithTime.ToShortDateString()} - {nextResetWithTime.ToShortDateString()})");
                embed.WithColor(new Color(raid.Color.R, raid.Color.G, raid.Color.B));

                var orderdUsersX = Users.users[guildId].Select(x => (user: x, completions: x.Completions.Values.Where(criteria).Count())).OrderByDescending(x => x.completions).ToList();
                orderdUsersWithRankX = orderdUsersX.Select(x => (x.user, x.completions, rank: orderdUsersX.IndexOf(x) + 1));
            }

            embed.AddField($"Completions {timespanText}", CreateLeaderboardString(orderdUsersWithRankX, userId, count, true), true);

            var users2 = Users.users[guildId].Where(x => x.Completions.Count > 0);
            Func<KeyValuePair<long, Completion>, int> getWeekNumberFromDate = y => Convert.ToInt32(Math.Floor((y.Value.Period - Bungie.ReleaseDate).TotalHours / hoursToDivideBy));
            Func<KeyValuePair<long, Completion>, bool> getCompletionNoRaid = z => z.Value.StartingPhaseIndex <= 0;
            var usersAndMaxPeriodCounts = users2.Select(user => (user, completions: user.Completions.GroupBy(getWeekNumberFromDate).Max(g => g.Where(getCompletionNoRaid).Count())));
            if (raid != null)
            {
                Func<KeyValuePair<long, Completion>, bool> getCompletionRaid = z => z.Value.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(z.Value.RaidHash) && z.Value.Time > raid.CompletionTime;
                usersAndMaxPeriodCounts = users2.Select(user => (user, completions: user.Completions.GroupBy(getWeekNumberFromDate).Max(g => g.Where(getCompletionRaid).Count())));
            }

            var orderedUsers = usersAndMaxPeriodCounts.OrderByDescending(u => u.completions).ToList();
            var orderedUsersWithRank = orderedUsers.Select(x => (x.user, x.completions, rank: orderedUsers.IndexOf(x) + 1));

            embed.AddField("-", "-", true);
            embed.AddField("Completion leaderboard", CreateLeaderboardString(orderedUsersWithRank, userId, count), true);
            return embed;
        }

        public static IEnumerable<(User, int, int)> GetListOfUsersWithCompletions(ulong guildId, DateTime startDate, DateTime endDate)
        {
            List<(User, int)> users = new List<(User, int)>();
            foreach(User user in Users.users[guildId])
            {
                int completions = 0;
                foreach(Raid raid in Raids.raids[guildId])
                {
                    completions += user.Completions.Values.Where(x => x.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && x.Time > raid.CompletionTime && raid.Hashes.Contains(x.RaidHash) && x.Period > startDate && x.Period < endDate).Count();
                }

                users.Add((user, completions));
            }

            return users.OrderByDescending(x => x.Item2).Select(x => (user: x.Item1, completions: x.Item2, rank: users.OrderByDescending(x => x.Item2).ToList().IndexOf(x) + 1));
        }

        public static string CreateLeaderboardString(IEnumerable<(User user, int completions, int rank)> users, ulong userDiscordId = 0, int count = 10, bool registerMessage = false)
        {
            string leaderboard = "";
            foreach((User user, int completions, int rank) user in users.Take(count))
            {
                if (user.user.DiscordID == userDiscordId) 
                { 
                    leaderboard += $"**{user.rank}) {user.user.Username}: {user.completions} completions** \n"; 
                    continue; 
                }

                leaderboard += $"{user.rank}) {user.user.Username}: {user.completions} completions \n";
            }

            foreach((User user, int completions, int rank) user in users.Where(x => x.user.DiscordID == userDiscordId))
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

            return leaderboard.Length <= 1024 ? leaderboard : "leaderboard string was too long.";
        }

        public static ulong GetTargetUser(SocketCommandContext context)
        {
            if (context.Message.MentionedUsers.Count == 0) return context.User.Id;

            if (Users.users[context.Guild.Id].Where(x => x.DiscordID == context.Message.MentionedUsers.FirstOrDefault().Id) != null) return context.Message.MentionedUsers.FirstOrDefault().Id;

            return 0;
        }
        
        public static ButtonStyle GetButtonStyleForPlatform(int membershipType)
        {
            return membershipType switch
            {
                1 => ButtonStyle.Success,
                2 => ButtonStyle.Primary,
                3 => ButtonStyle.Danger,
                _ => ButtonStyle.Secondary
            };
        }

        public static string GetErrorCodeForUserSearch(RequestData requestData)
        {
            return requestData.Code switch
            {
                2 => "Please enter an ID or register.",
                3 => "Please enter a valid membership type.",
                4 => "User not found",
                5 => "Please enter a valid Steam Id.",
                6 => "Please enter a valid BungieID",
                _ => requestData.DisplayName
            };
        }
    }
}
