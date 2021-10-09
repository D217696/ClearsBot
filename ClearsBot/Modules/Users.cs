using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Users
    {
        private const string configFolder = "Resources";
        private const string configFile = "users.json";
        public Dictionary<ulong, List<User>> users = new Dictionary<ulong, List<User>>();
        private List<User> usersToUpdate = new List<User>();
        public bool busy = false;
        readonly IBungieDestiny2RequestHandler _requestHandler;
        readonly IBungie _bungie;
        readonly IUtilities _utilities;
        readonly IGuilds _guilds;
        readonly IRaids _raids;
        readonly Roles _roles;
        public Users(IBungieDestiny2RequestHandler requestHandler, IBungie bungie, IUtilities utilities, IGuilds guilds, IRaids raids, Roles roles)
        {
            _requestHandler = requestHandler;
            _bungie = bungie;
            _utilities = utilities;
            _guilds = guilds;
            _raids = raids;
            _roles = roles;
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(users, Formatting.Indented));
            }
            else
            {
                string UsersString = File.ReadAllText(configFolder + "/" + configFile);
                if (UsersString == "")
                {
                    users = new Dictionary<ulong, List<User>>();
                }
                else
                {
                    users = JsonConvert.DeserializeObject<Dictionary<ulong, List<User>>>(UsersString);
                }
            }

            foreach (SocketGuild guild in Program._client.Guilds)
            {
                if (users.ContainsKey(guild.Id)) continue;
                users.Add(guild.Id, new List<User>());
                SaveUsers();
            }

            new Thread(new ThreadStart(UpdateUsers)).Start();
        }
        async void UpdateUsers()
        {
            while (true)
            {
                if (DateTime.Now.Minute % 5 == 0) await AddUsersToUpdateUsersList();
                if (DateTime.Now.Minute % 30 == 0) _ = UpdateUsersAsync();
                if (DateTime.Now.Minute % 30 == 0) _ = _roles.UpdateRolesForGuildsAsync();
                Thread.Sleep(1000 * 60);
            }
        }
        public async Task RegisterUser(ISocketMessageChannel channel, ulong guildId, ulong userId, string discordUsername, string membershipId = "", string membershipType = "", RestFollowupMessage restFollowupMessage = null)
        {
            busy = true;
            if (membershipId == "")
            {
                await channel.SendMessageAsync("Usage: /register (Steam Id | Membership Id) (Membership Type)");
                busy = false;
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
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription(_utilities.GetErrorCodeForUserSearch(requestData)).Build());
                busy = false;
                return;
            }
            else if (requestData.Code == 8)
            {
                int buttonCount = 0;
                int buttonRow = 0;
                var componentBuilder = new ComponentBuilder();
                foreach (UserInfoCard userInfoCard in requestData.profiles)
                {
                    ButtonStyle buttonStyle = _utilities.GetButtonStyleForPlatform(userInfoCard.MembershipType);

                    componentBuilder.WithButton(new ButtonBuilder().WithLabel(userInfoCard.DisplayName).WithCustomId($"register_{userInfoCard.MembershipId}_{userInfoCard.MembershipType}").WithStyle(buttonStyle), buttonRow);
                    buttonCount++;
                    if (buttonCount % 5 == 0) buttonRow++;
                }
                await restUserMessage.ModifyAsync(x => x.Components = componentBuilder.Build());
                await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("Multiple profiles found, select correct one below").Build());
                busy = false;
                return;
            }

            UserResponse userResponse = await CreateAndAddUser(guildId, userId, requestData);
            switch (userResponse.Code)
            {
                case 1:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("User created. - Getting Activities.").Build());
                    busy = false;

                    break;
                //case 2:
                //    await restUserMessage.ModifyAsync(x => x.Content = "You already have an account linked to your discord.");
                //    break;
                case 3:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("There's already a discord account linked to this profile.").Build());
                    busy = false;

                    return;
                case 4:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription("There's already a discord account linked to this profile.").Build());
                    busy = false;
                    return;
            }

            userResponse.User.DateLastPlayed = new DateTime(2017, 08, 06, 0, 0, 0);
            GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(userResponse.User);
            switch (getCompletionsResponse.Code)
            {
                case 1:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Activities found: {users[guildId].Where(x => x.DiscordID == userId && x.MembershipId == requestData.MembershipId).FirstOrDefault().Completions.Count} raid completions").Build());
                    break;
                default:
                    await restUserMessage.ModifyAsync(x => x.Embed = embed.WithDescription($"Something went wrong: {getCompletionsResponse.ErrorMessage}").Build());
                    break;
            }

            busy = false;
        }
        public ulong GetTargetUser(SocketCommandContext context)
        {
            if (context.Message.MentionedUsers.Count == 0) return context.User.Id;

            if (users[context.Guild.Id].Where(x => x.DiscordID == context.Message.MentionedUsers.FirstOrDefault().Id) != null) return context.Message.MentionedUsers.FirstOrDefault().Id;

            return 0;
        }
        public IEnumerable<(User user, int completions, int rank)> GetListOfUsersWithCompletions(ulong guildId, DateTime startDate, DateTime endDate, Raid raid)
        {
            if (raid != null)
            {
                List<(User user, int completions)> usersList = users[guildId].Select(x => (user: x, completions: x.Completions.Values.Where(_raids.GetCriteriaByRaid(raid)).Where(x => x.Period > startDate && x.Period < endDate).Count())).ToList().OrderByDescending(x => x.completions).ToList();
                return usersList.Select(x => (x.user, x.completions, rank: usersList.IndexOf(x) + 1));
            }

            List<(User user, int completions)> userList = new List<(User, int)>();
            foreach (User user in users[guildId])
            {
                int completions = 0;
                foreach (Raid localRaid in _raids.GetRaids(guildId))
                {
                    completions += user.Completions.Values.Where(_raids.GetCriteriaByRaid(localRaid)).Where( x => x.Period > startDate && x.Period < endDate).Count();
                }
                userList.Add((user, completions));
            }

            return userList.OrderByDescending(x => x.user).Select(x => (x.user, x.completions, rank: userList.OrderByDescending(x => x.completions).ToList().IndexOf(x) + 1));
        }
        public IEnumerable<User> GetUsersByPage(ulong guildId, int index = 1)
        {
            return users[guildId].Where(x => users[guildId].IndexOf(x) <= (index * 10) - 1 && users[guildId].IndexOf(x) > (index * 10) - 11);
        }
        public async Task AddUsersToUpdateUsersList()
        {
            try
            {
                bool loopNotDone = true;
                while (!busy && loopNotDone)
                {
                    foreach (var guild in users)
                    {
                        foreach (User user in guild.Value)
                        {
                            if (user.GuildID == 0) user.GuildID = guild.Key;
                            if (DateTime.UtcNow - user.DateLastPlayed > new TimeSpan(1, 0, 0))
                            {
                                if (usersToUpdate.Contains(user)) continue;
                                usersToUpdate.Add(user);
                            }
                        }
                    }
                    loopNotDone = false;
                }
            }
            catch (Exception)
            {

            }
        }
        public async Task UpdateUsersAsync()
        {
            foreach(User user in usersToUpdate)
            {
                GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(user);
                if (getCompletionsResponse.Code == 1)
                {
                    if (users[getCompletionsResponse.User.GuildID].FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId) != null)
                    {
                        users[getCompletionsResponse.User.GuildID].FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).Completions = getCompletionsResponse.User.Completions;
                        users[getCompletionsResponse.User.GuildID].FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).Characters = getCompletionsResponse.User.Characters;
                        users[getCompletionsResponse.User.GuildID].FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).DateLastPlayed = getCompletionsResponse.User.DateLastPlayed;
                    }
                }
                else
                {
                    //await labsDMs.SendMessageAsync("Couldnt update users. " + getCompletionsResponse.ErrorMessage);
                    return;
                }
            }
            usersToUpdate = new List<User>();
            SaveUsers();
        }
        public async Task GiveRoleToUser(IGuildUser user, IRole role, IReadOnlyCollection<IGuildUser> users)
        {
            if (role == null) return;
            bool userHasRole = false;
            foreach (IGuildUser roleUser in users.Where(x => x.RoleIds.Contains(role.Id)))
            {
                if (roleUser == user && user != null)
                {
                    userHasRole = true;
                    continue;
                }
                await roleUser.RemoveRoleAsync(role);
                Console.WriteLine($"Removed {role.Name} from {roleUser.Username}");
            }

            if (user == null) return;
            if (!userHasRole) {
                await user.AddRoleAsync(role);
                Console.WriteLine($"Added {role.Name} to {user.Username}");
            } 
        }
        public void SaveUsers()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(users, Formatting.Indented));
        }

        public List<User> GetGuildUsers(ulong guildId)
        {
            return users[guildId];
        }
        public List<User> GetUsers(SocketCommandContext context)
        {
            ulong userId = context.Message.MentionedUsers.Count == 0 ? context.User.Id : context.Message.MentionedUsers.FirstOrDefault().Id;
            return users[context.Guild.Id].Where(x => x.DiscordID == userId).ToList();
        }
        public List<User> GetUsers(ulong guildId, ulong userId)
        {
            return users[guildId].Where(x => x.DiscordID == userId).ToList();
        }
        public async Task<UserResponse> CreateAndAddUser(ulong guildID, ulong discordID, RequestData requestData)
        {
            //if (users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault(), Code = 2 };
            if (users[guildID].Where(x => x.SteamID == requestData.SteamID).FirstOrDefault() != null && requestData.SteamID != 0) return new UserResponse() { User = users[guildID].Where(x => x.SteamID == requestData.SteamID).FirstOrDefault(), Code = 3 };
            if (users[guildID].Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault(), Code = 4 };

            Bungie bungie = new Bungie(new BungieDestiny2RequestHandler(new Logger()));
            GetHistoricalStatsForAccount getHistoricalStatsForAccount = await _requestHandler.GetHistoricalStatsForAccount(requestData.MembershipType, requestData.MembershipId);
            if (getHistoricalStatsForAccount.ErrorCode != 1) return new UserResponse() { User = null, Code = 5 };

            List<Character> characters = new List<Character>();
            foreach (DestinyHistoricalStatsPerCharacter character in getHistoricalStatsForAccount.Response.Characters)
            {
                characters.Add(new Character()
                {
                    CharacterID = character.CharacterId,
                    Deleted = character.Deleted,
                    Handled = false
                });
            }

            User user = new User()
            {
                Username = requestData.DisplayName,
                DiscordID = discordID,
                SteamID = requestData.SteamID,
                MembershipId = requestData.MembershipId,
                MembershipType = requestData.MembershipType,
                DateLastPlayed = requestData.DateLastPlayed,
                Characters = characters,
                DateRegistered = DateTime.UtcNow
            };

            users[guildID].Add(user);
            SaveUsers();
            return new UserResponse() { User = user, Code = 1 };
        }
    }
    public class UserResponse
    {
        public User User { get; set; }
        public int Code { get; set; }
    }
}
