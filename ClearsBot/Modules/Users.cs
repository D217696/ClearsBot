using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClearsBot.Modules
{
    public class Users
    {
        private List<User> users = new List<User>();
        private List<User> usersToUpdate = new List<User>();
        readonly IBungieDestiny2RequestHandler _requestHandler;
        readonly IBungie _bungie;
        readonly IRaids _raids;
        readonly IStorage _storage;
        public Users(IBungieDestiny2RequestHandler requestHandler, IBungie bungie, IRaids raids, IStorage storage)
        {
            _requestHandler = requestHandler;
            _bungie = bungie;
            _raids = raids;
            _storage = storage;

            users = _storage.GetUsersFromStorage();
            Console.WriteLine($"{users.Count()}");
        }
        
        public async Task GuildJoined(ulong guildId)
        {
            _ = SyncUsers(guildId);
        }
        public async Task SyncUsers(ulong guildId)
        {
            SocketGuild guild = Program._client.Guilds.FirstOrDefault(x => x.Id == guildId);
            foreach (IGuildUser guildUser in await guild.GetUsersAsync().FlattenAsync())
            {
                foreach (User user in users.Where(x => x.DiscordID == guildUser.Id))
                {
                    user.GuildIDs.Add(guildId);
                }
            }
        }
        public ulong GetTargetUser(SocketCommandContext context)
        {
            if (context.Message.MentionedUsers.Count == 0) return context.User.Id;

            if (users.Where(x => x.GuildIDs.Contains(context.Guild.Id)).Where(x => x.DiscordID == context.Message.MentionedUsers.FirstOrDefault().Id) != null) return context.Message.MentionedUsers.FirstOrDefault().Id;

            return 0;
        }
        public IEnumerable<User> GetUsersByPage(ulong guildId, int index = 1)
        {
            return users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => users.Where(x => x.GuildIDs.Contains(guildId)).ToList().IndexOf(x) <= (index * 10) - 1 && users.Where(x => x.GuildIDs.Contains(guildId)).ToList().IndexOf(x) > (index * 10) - 11);
        }
        public void AddUsersToUpdateUsersList()
        {
            var tempUsers = new List<User>(users);
            foreach (User user in tempUsers)
            {
                if (DateTime.UtcNow - user.DateLastPlayed > new TimeSpan(1, 0, 0))
                { 
                    if (usersToUpdate.Contains(user)) continue;
                    usersToUpdate.Add(user);
                }
            }
        }
        public async Task UpdateUsersAsync()
        {
            List<Task> updateTasks = new List<Task>();
            var temp = new List<User>(usersToUpdate);
            foreach(User user in temp)
            {
                updateTasks.Add(Task.Run(() => UpdateUser(user)));
            }
            usersToUpdate = new List<User>();
            await Task.WhenAll(updateTasks);
            SaveUsers();
        }
        private async Task UpdateUser(User user)
        {
            GetCompletionsResponse getCompletionsResponse = await _bungie.GetCompletionsForUserAsync(user);
            if (getCompletionsResponse.Code == 1)
            {
                if (users.FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId) != null)
                {
                    users.FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).Completions = getCompletionsResponse.User.Completions;
                    users.FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).Characters = getCompletionsResponse.User.Characters;
                    users.FirstOrDefault(x => x.MembershipId == getCompletionsResponse.User.MembershipId).DateLastPlayed = getCompletionsResponse.User.DateLastPlayed;
                }
            }
        }
        public void SaveUsers()
        {
            _storage.SaveUsers(users);
        }
        public List<User> GetAllUsers()
        {
            return users;
        }
        public List<User> GetGuildUsers(ulong guildId)
        {
            return users.Where(x => x.GuildIDs.Contains(guildId)).ToList();
        }
        public List<User> GetUsers(SocketCommandContext context)
        {
            ulong userId = context.Message.MentionedUsers.Count == 0 ? context.User.Id : context.Message.MentionedUsers.FirstOrDefault().Id;
            return users.Where(x => x.GuildIDs.Contains(context.Guild.Id)).Where(x => x.Completions.Count > 0).Where(x => x.DiscordID == userId).ToList();
        }
        public User GetUserByMembershipId(long membershipId)
        {
            return users.FirstOrDefault(x => x.MembershipId == membershipId);
        }
        public List<User> GetUsers(ulong guildId, ulong userId)
        {
            return users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.DiscordID == userId).ToList();
        }
        public IEnumerable<User> GetUsersByDiscordId(ulong userId)
        {
            return users.Where(x => x.DiscordID == userId);
        }

        public async Task<UserResponse> CreateAndAddUser(ulong guildId, ulong discordID, RequestData requestData)
        {
            //if (users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault(), Code = 2 };
            if (users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.SteamID == requestData.SteamID).FirstOrDefault() != null && requestData.SteamID != 0) return new UserResponse() { User = users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.SteamID == requestData.SteamID).FirstOrDefault(), Code = 3 };
            if (users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault() != null) return new UserResponse() { User = users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.MembershipId == requestData.MembershipId).FirstOrDefault(), Code = 4 };

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
                DateRegistered = DateTime.UtcNow,
                GuildIDs = new List<ulong>() { guildId }
            };

            users.Add(user);
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
