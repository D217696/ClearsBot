using ClearsBot.Objects;
using Discord;
using Discord.Commands;
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
    public static class Users
    {
        public static IDMChannel labsDMs;
        private const string configFolder = "Resources";
        private const string configFile = "users.json";
        public static Dictionary<ulong, List<User>> users = new Dictionary<ulong, List<User>>();
        private static List<User> usersToUpdate = new List<User>();
        public static bool busy = false;
        public static Bungie bungie = new Bungie();

        public static async Task Initialize()
        {
            var user = await Program._client.Rest.GetUserAsync(204722865818304512);
            //await user.SendMessageAsync("init done!");
            labsDMs = await user.CreateDMChannelAsync();
            await labsDMs.SendMessageAsync("init done!");

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

        static async void UpdateUsers()
        {
            while (true)
            {
                if (DateTime.Now.Minute % 5 == 0) await AddUsersToUpdateUsersList();
                if (DateTime.Now.Minute % 30 == 0) _ = UpdateUsersAsync();
                if (DateTime.Now.Minute % 30 == 0) _ = UpdateRolesForGuildsAsync();
                Thread.Sleep(1000 * 60);
            }
        }

        public static async Task AddUsersToUpdateUsersList()
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
            catch(Exception ex)
            {

            }
        }

        public static async Task UpdateUsersAsync()
        {
            foreach(User user in usersToUpdate)
            {
                GetCompletionsResponse getCompletionsResponse = await bungie.GetCompletionsForUserAsync(user);
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
                    await labsDMs.SendMessageAsync("Couldnt update users. " + getCompletionsResponse.ErrorMessage);
                    return;
                }
            }
            usersToUpdate = new List<User>();
            SaveUsers();
        }

        public static async Task UpdateRolesForGuildsAsync()
        {
            foreach(Guild guild in Guilds.guilds.Values)
            {
                foreach(Raid raid in Raids.raids[guild.GuildId])
                {
                    List<(User, int, int)> users = Misc.GetListOfUsersWithCompletions(guild.GuildId, Bungie.ReleaseDate, DateTime.UtcNow, raid).ToList();

                    await GiveRoleToUser(Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetUser(users[0].Item1.DiscordID), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetRole(raid.FirstRole), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).Users);
                    await GiveRoleToUser(Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetUser(users[1].Item1.DiscordID), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetRole(raid.SecondRole), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).Users);
                    await GiveRoleToUser(Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetUser(users[2].Item1.DiscordID), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).GetRole(raid.ThirdRole), Program._client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId).Users);
                }
            }
        }
        
        public static async Task GiveRoleToUser(IGuildUser user, IRole role, IReadOnlyCollection<IGuildUser> users)
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

        public static void SaveUsers()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(users, Formatting.Indented));
        }

        public static List<User> GetUsers(SocketCommandContext context)
        {
            ulong userId = context.Message.MentionedUsers.Count == 0 ? context.User.Id : context.Message.MentionedUsers.FirstOrDefault().Id;
            return users[context.Guild.Id].Where(x => x.DiscordID == userId).ToList();
        }

        public static List<User> GetUsers(ulong guildId, ulong userId)
        {
            return users[guildId].Where(x => x.DiscordID == userId).ToList();
        }

        public async static Task<UserResponse> CreateAndAddUser(ulong guildID, ulong discordID, RequestData requestData)
        {
            //if (users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault(), Code = 2 };
            if (users[guildID].Where(x => x.SteamID == requestData.SteamID).FirstOrDefault() != null && requestData.SteamID != 0) return new UserResponse() { User = users[guildID].Where(x => x.SteamID == requestData.SteamID).FirstOrDefault(), Code = 3 };
            if (users[guildID].Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault(), Code = 4 };

            Bungie bungie = new Bungie();
            GetHistoricalStatsForAccount getHistoricalStatsForAccount = JsonConvert.DeserializeObject<GetHistoricalStatsForAccount>(await bungie.MakeRequest($"Platform/Destiny2/{requestData.MembershipType}/Account/{requestData.MembershipId}/Stats/"));
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
