using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        readonly IBungie _bungie;
        readonly Users _users;
        readonly IUtilities _utilities;
        readonly IPermissions _permissions;
        readonly IGuilds _guilds;
        readonly Commands _commands;
        readonly IRaids _raids;
        public Misc(IBungie bungie, Users users, IUtilities utilities, IPermissions permissions, IGuilds guilds, Commands commands, IRaids raids)
        {
            _bungie = bungie;
            _users = users;
            _utilities = utilities;
            _permissions = permissions;
            _guilds = guilds;
            _commands = commands;
            _raids = raids;
        }

        //leaderboard commands
        [Command("Rank")]
        public async Task Rank([Remainder] string raidString = "")
        {
            ulong targetUserId = _users.GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: _commands.RankCommand(targetUserId, Context.Guild.Id, raidString).Build());
        }

        [Command("Yearly")]
        public async Task Yearly([Remainder] string raidString = "")
        {
            ulong targetUserId = _users.GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: _commands.TimeFrameCommand(targetUserId, Context.Guild.Id, raidString, TimeFrameHours.Year, "Yearly").Build());
        }

        [Command("Monthly")]
        public async Task Monthly([Remainder] string raidString = "")
        {
            ulong targetUserId = _users.GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: _commands.TimeFrameCommand(targetUserId, Context.Guild.Id, raidString, TimeFrameHours.Month, "Monthly").Build());
        }

        [Command("Weekly")]
        public async Task Weekly([Remainder] string raidString = "")
        {
            ulong targetUserId = _users.GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: _commands.TimeFrameCommand(targetUserId, Context.Guild.Id, raidString, TimeFrameHours.Week, "Weekly").Build());
        }

        [Command("Daily")]
        public async Task Daily([Remainder] string raidString = "")
        {
            ulong targetUserId = _users.GetTargetUser(Context);
            if (targetUserId == 0)
            {
                await ReplyAsync("Targeted user has not registered");
                return;
            }

            await ReplyAsync(embed: _commands.TimeFrameCommand(targetUserId, Context.Guild.Id, raidString, TimeFrameHours.Day, "Daily").Build());
        }

        //raid commands
        [Command("SetTime")]
        public async Task SetTime(string raidString = "", string timeString = "")
        {
            await ReplyAsync(_commands.EditRaidTimeCommand(Context.Guild.GetUser(Context.User.Id), Context.Guild.Id, raidString, timeString));
        }

        [Command("AddShortcut")]
        public async Task AddShortcut(string raidString = "", string shortcut = "")
        {
            await ReplyAsync(_commands.AddRaidShortcutCommand(Context.Guild.GetUser(Context.User.Id), Context.Guild.Id, raidString, shortcut));
        }

        [Command("Raids")]
        public async Task ShowRaids()
        {
            await ReplyAsync(embed: _commands.DisplayRaidsCommand(Context.Guild.Id).Build());
        }


        //Commands below need revision to be compliant with SOLID
        [Command("Register", RunMode = RunMode.Async)]
        public async Task Register([Remainder] string membershipId = "")
        {
            await _users.RegisterUser(Context.Channel, Context.Guild.Id, Context.User.Id, Context.User.Username, membershipId, "");
        }

        [Command("Completions")]
        public async Task Completions()
        {
            List<User> users = _users.GetUsers(Context);
            if (users.Count == 0)
            {
                await Context.Channel.SendMessageAsync("You have not registered.");
                return;
            }

            await ReplyAsync(embed: _utilities.GetCompletionsForUser(users.FirstOrDefault(), Context.Guild.Id).Build(), component: _utilities.GetButtonsForUser(users, Context.Guild.Id, "completions", users.FirstOrDefault()).Build());
        }

        [Command("Update", RunMode = RunMode.Async)]
        public async Task Update()
        {
            string description = "";
            foreach (User user in _users.users[Context.Guild.Id].Where(x => x.DiscordID == Context.User.Id))
            {
                GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(user);
                if (getCompletionsResponse.Code != 1)
                {
                    await Context.Channel.SendMessageAsync(getCompletionsResponse.ErrorMessage);
                    return;
                }
                description += $"{user.Username}: {user.Completions.Count} pgcrs saved. \n";
            }

            _users.SaveUsers();
            await ReplyAsync(description);
        }


        [Command("Profiles")]
        public async Task Profiles([Remainder] string remainder = "")
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Profiles linked to your account");
            List<User> users = _users.GetUsers(Context);
            foreach (User user in users)
            {
                string platform = Bungie.GetPlatformString(user.MembershipType);
                embed.AddField(user.Username, $"{platform} \n Characters: {user.Characters.Count()} \n Saved pgcrs: {user.Completions.Count()} \n Date last played: {user.DateLastPlayed} \n SteamID: {user.SteamID}", true);
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("GenerateRoles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task GenerateRoles()
        {
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminUser)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            var embed = new EmbedBuilder();
            foreach (Raid raid in _raids.GetRaids(Context.Guild.Id))
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

            _guilds.GetGuild(Context.Guild.Id).FirstRole = firstRoleTotal.Id;
            _guilds.GetGuild(Context.Guild.Id).SecondRole = secondRoleTotal.Id;
            _guilds.GetGuild(Context.Guild.Id).ThirdRole = thirdRoleTotal.Id;

            embed.AddField("Total", $"{firstRoleTotal.Mention} \n {secondRoleTotal.Mention} \n {thirdRoleTotal.Mention}");

            _guilds.SaveGuilds();
            //_raids.SaveRaids();

            await ReplyAsync(embed: embed.Build());
        }

        [Command("SetPrefix")]
        public async Task SetPrefix(string prefix)
        {
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            _guilds.GetGuild(Context.Guild.Id).Prefix = prefix;
            _guilds.SaveGuilds();
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
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync("No permission");
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("Profiles linked to account");
            List<User> users = _users.GetUsers(Context);
            foreach (User user in users)
            {
                string platform = Bungie.GetPlatformString(user.MembershipType);
                embed.AddField(user.Username, $"{platform} \n Characters: {user.Characters.Count()} \n Saved pgcrs: {user.Completions.Count()} \n Date last played: {user.DateLastPlayed} \n SteamID: {user.SteamID}", true);
            }

            await ReplyAsync(embed: embed.Build(), component: _utilities.GetButtonsForUser(users, Context.Guild.Id, "unregister").Build());
        }

        //[Command("reglist")]
        //public async Task GetUsers()
        //{
        //    var embed = new EmbedBuilder
        //    {
        //        Title = $"Users page (1/{Math.Ceiling((decimal)_users.users[Context.Guild.Id].Count() / 10)} total users: {_users.users[Context.Guild.Id].Count()})"
        //    };

        //    foreach (User user in _users.GetUsersByPage(Context.Guild.Id))
        //    {
        //        embed.Description += $"<@!{user.DiscordID}>: {user.Username} \n";
        //    }

        //    if (Math.Ceiling((double)_users.users[Context.Guild.Id].Count() / 10) > 1)
        //    {
        //        var componentBuilder = new ComponentBuilder();

        //        componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Danger).WithLabel("next").WithCustomId($"reglist_{Context.Guild.Id}_2_{Context.User.Id}"));
        //        await ReplyAsync(embed: embed.Build(), component: componentBuilder.Build());
        //        return;
        //    }

        //    await ReplyAsync(embed: embed.Build());

        //    //await ReplyAsync($"Number of bungie profiles being tracked: **{Users.users[Context.Guild.Id].Count()}** \nNumber of different discord users being tracked: **{Users.users[Context.Guild.Id].GroupBy(x => x.DiscordID).Count()}**");
        //}

        [Command("CreateMilestone")]
        public async Task CreateMilestone(string completions, string role)
        {
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminRole)
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

            _guilds.GetGuild(Context.Guild.Id).Milestones.Add(new Milestone()
            {
                Completions = completionCount,
                Role = socketRole.Id
            });

            _guilds.SaveGuilds();
            await ReplyAsync($"Milestone created for {completionCount} completions, {socketRole.Mention}");
        }

        [Command("Admin")]
        public async Task AddAdmin(string role)
        {
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminUser)
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

            _guilds.GetGuild(Context.Guild.Id).AdminRole = socketRole.Id;
            _guilds.SaveGuilds();
            await ReplyAsync($"{socketRole.Mention} set as Admin role");
        }

        [Command("Mod")]
        public async Task AddMod(string role)
        {
            if (_permissions.GetPermissionForUser(Context.Guild.GetUser(Context.User.Id)) < PermissionLevels.AdminRole)
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

            _guilds.GetGuild(Context.Guild.Id).ModRoles.Add(socketRole.Id);
            _guilds.SaveGuilds();
            await ReplyAsync($"{socketRole.Mention} set as mod role");
        }

        [Command("Verify", RunMode = RunMode.Async)]
        public async Task Verify()
        {
            List<User> users = _users.GetUsers(Context);
            var message = await ReplyAsync("Verifying your activities..");
            int completions = 0;
            int newCompletions = 0;
            foreach (User user in users)
            {
                completions += user.Completions.Count;
                user.DateLastPlayed = new DateTime(2017, 01, 01, 0, 0, 0);
                foreach (Character character in user.Characters)
                {
                    character.Deleted = false;
                    character.Handled = false;
                }
                await _bungie.GetCompletionsForUserAsync(user);
                newCompletions += user.Completions.Count;
            }

            await message.ModifyAsync(x => x.Content = $"Found {newCompletions - completions} new activities");
        }

        //[Command("Fastest")]
        //public async Task Fastest(string raidString = "")
        //{
        //    Raid raid = null;
        //    if(raidString != "")
        //    {
        //        raid = Raids.raids[Context.Guild.Id].FirstOrDefault(x => x.Shortcuts.Contains(raidString) || x.DisplayName.ToLower().Contains(raidString));
        //    }

        //    ulong targetUserId = _users.GetTargetUser(Context);

        //    await ReplyAsync(embed: _utilities.GetFastestListForUser(_users.users[Context.Guild.Id].FirstOrDefault(x => x.DiscordID == targetUserId), raid, Context.Guild.Id).Build(), component: _utilities.GetButtonsForUser(_users.users[Context.Guild.Id].Where(x => x.DiscordID == targetUserId).ToList(), Context.Guild.Id, "fastest", _users.users[Context.Guild.Id].FirstOrDefault(x => x.DiscordID == targetUserId), raidString).Build());
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
    }
}
