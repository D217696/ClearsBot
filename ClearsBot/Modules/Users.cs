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
            await user.SendMessageAsync("init done!");
            //labsDMs = await user.CreateDMChannelAsync();
            //await labsDMs.SendMessageAsync("init done!");

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
                if (busy)
                {
                    Console.WriteLine("skipped updating");
                    Thread.Sleep(1000 * 60 * 5);
                    continue;
                }

                Console.WriteLine("Updating users");
                await UpdateUsersLoop();
                Console.WriteLine("Done updating");
                Thread.Sleep(1000 * 60 * 30);
            }
        }

        public static async Task UpdateUsersLoop()
        {
            bool loopNotDone = true;

            while (!busy && loopNotDone)
            {

            }
            //while (!busy && loopNotDone)
            //{

                //    //Dictionary<ulong, List<User>> usersCopy = new Dictionary<ulong, List<User>>(users);
                //    //foreach (KeyValuePair<ulong, List<User>> guild in usersCopy)
                //    //{
                //    //    Guild guildFromBot = Guilds.guilds[guild.Key];
                //    //    SocketGuild socketGuild = Program._client.Guilds.Where(x => x.Id == guild.Key).FirstOrDefault();
                //    //    var usersFromGuild = await((IGuild)socketGuild).GetUsersAsync();

                //    //    foreach (User user in guild.Value)
                //    //    {
                //    //        if (busy) continue;
                //    //        _ = await bungie.GetCompletionsForUserAsync(guild.Key, user.DiscordID, user.MembershipId);
                //    //        SaveUsers();
                //    //        Console.WriteLine("updated: " + user.Username);
                //    //        int completions = 0;
                //    //        foreach (Raid raid in Raids.raids[guild.Key])
                //    //        {
                //    //            completions += user.Completions.Where(x => x.Value.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && raid.Hashes.Contains(x.Value.RaidHash) && x.Value.Time >= raid.CompletionTime).Count();
                //    //        }

                //    //        bool milestoneReached = false;
                //    //        if (guildFromBot.Milestones == null) continue;
                //    //        foreach (Milestone milestone in guildFromBot.Milestones.OrderByDescending(x => x.Completions).ToList())
                //    //        {
                //    //            if (milestoneReached) continue;
                //    //            var userFromGuild = usersFromGuild.Where(x => x.Id == user.DiscordID).FirstOrDefault();
                //    //            if (userFromGuild == null) continue;
                //    //            foreach (Milestone milestone1 in guildFromBot.Milestones.OrderByDescending(x => x.Completions).ToList())
                //    //            {
                //    //                if (milestone1.Role == milestone.Role) continue;
                //    //                if (!userFromGuild.RoleIds.Contains(milestone1.Role)) continue;
                //    //                SocketRole milestoneRole = socketGuild.GetRole(milestone1.Role);
                //    //                await userFromGuild.RemoveRoleAsync(milestoneRole);
                //    //                Console.WriteLine($"Removed {milestoneRole.Name} from {userFromGuild.Username}");
                //    //            }

                //    //            if (completions >= milestone.Completions)
                //    //            {
                //    //                SocketRole milestone2Role = socketGuild.GetRole(milestone.Role);
                //    //                if (userFromGuild.RoleIds.Contains(milestone.Role)) continue;
                //    //                await userFromGuild.AddRoleAsync(milestone2Role);
                //    //                Console.WriteLine($"Added {milestone2Role.Name} to {userFromGuild.Username}");
                //    //                milestoneReached = true;
                //    //            }
                //    //        }
                //    //    }

                //    //    foreach (Raid raid in Raids.raids[guild.Key])
                //    //    {
                //    //        List<User> users = Misc.GetListOfUsersSorted(raid, guild.Key);

                //    //        if (users.Count < 1) continue;
                //    //        await GiveRoleToUser(usersFromGuild.Where(x => x.Id == users[0].DiscordID).FirstOrDefault(), socketGuild.GetRole(raid.FirstRole), usersFromGuild);
                //    //        if (users.Count < 2) continue;
                //    //        await GiveRoleToUser(usersFromGuild.Where(x => x.Id == users[1].DiscordID).FirstOrDefault(), socketGuild.GetRole(raid.SecondRole), usersFromGuild);
                //    //        if (users.Count < 3) continue;
                //    //        await GiveRoleToUser(usersFromGuild.Where(x => x.Id == users[2].DiscordID).FirstOrDefault(), socketGuild.GetRole(raid.ThirdRole), usersFromGuild);
                //    //    }

                //    //    List<User> totalUsers = Misc.GetListOfUsersSorted(null, guild.Key);
                //    //    if (totalUsers.Count < 1) continue;
                //    //    await GiveRoleToUser(usersFromGuild.Where(x => x.Id == totalUsers[0].DiscordID).FirstOrDefault(), socketGuild.GetRole(guildFromBot.FirstRole), usersFromGuild);
                //    //    if (totalUsers.Count < 2) continue;
                //    //    await GiveRoleToUser(usersFromGuild.Where(x => x.Id == totalUsers[1].DiscordID).FirstOrDefault(), socketGuild.GetRole(guildFromBot.SecondRole), usersFromGuild);
                //    //    if (totalUsers.Count < 3) continue;
                //    //    await GiveRoleToUser(usersFromGuild.Where(x => x.Id == totalUsers[2].DiscordID).FirstOrDefault(), socketGuild.GetRole(guildFromBot.ThirdRole), usersFromGuild);
                //    //}
                //}
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
                Characters = characters
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
