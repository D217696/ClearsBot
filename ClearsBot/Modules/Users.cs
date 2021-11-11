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
using ClearsBot.Modules;

namespace ClearsBot.Modules
{
    public class Users
    {
        private List<User> users = new List<User>();
        readonly IBungieDestiny2RequestHandler _requestHandler;
        readonly IBungie _bungie;
        readonly IRaids _raids;
        readonly IGuilds _guilds;
        readonly IStorage _storage;
        readonly DiscordSocketClient _client;
        public Users(IBungieDestiny2RequestHandler requestHandler, IBungie bungie, IRaids raids, IStorage storage, IGuilds guilds, DiscordSocketClient client)
        {
            _requestHandler = requestHandler;
            _bungie = bungie;
            _raids = raids;
            _storage = storage;
            _guilds = guilds;
            _client = client;
            // remove select after first time running
            users = _storage.GetUsersFromStorage().Select(user => new User()
            {
                Username = user.Username,
                DiscordID = user.DiscordID,
                MembershipId = user.MembershipId,
                MembershipType = user.MembershipType,
                DateLastPlayed = user.DateLastPlayed,
                Completions = user.Completions,
                Guid = user.Guid.ToString() != "00000000-0000-0000-0000-000000000000" ? user.Guid : Guid.NewGuid(),
                DateRegistered = user.DateRegistered,
                GuildID = user.GuildID,
                GuildIDs = user.GuildIDs,
                SteamID = user.SteamID,
                Characters = user.Characters
            }).ToList();
            SaveUsers();
            Console.WriteLine($"{users.Count()}");
        }
        
        public async Task SyncUsers(ulong guildId)
        {
            SocketGuild guild = _client.Guilds.FirstOrDefault(x => x.Id == guildId);
            if (guild == null) return;
            if (_guilds.GetGuild(guildId) == null) return;
            if (!_guilds.GetGuild(guildId).IsActive) return;

            foreach (IGuildUser guildUser in await guild.GetUsersAsync().FlattenAsync())
            {
                foreach (User user in users.Where(x => x.DiscordID == guildUser.Id))
                {
                    if (!user.GuildIDs.Contains(guildId)) user.GuildIDs.Add(guildId);
                    user.GuildIDs = user.GuildIDs.GroupBy(x => x).SelectMany(x => x).ToList(); // line can be taken out in the future
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
        public async Task UpdateUsersAsync()
        {
            List<Task> updateTasks = new List<Task>();
            var temp = new List<User>(users);
            foreach(User user in temp)
            {
                //await UpdateUser(user);
                updateTasks.Add(Task.Run(() => UpdateUser(user)));
            }
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

        public void DeleteUserByGuid(Guid guid)
        {
            if (users.FirstOrDefault(x => x.Guid == guid) == null) return;
            users.Remove(users.FirstOrDefault(x => x.Guid == guid));
        }

        public void ResetUserCompletions(User user)
        {
            users.FirstOrDefault(x => x.MembershipId == user.MembershipId).Completions = new Dictionary<long, Completion>();
            users.FirstOrDefault(x => x.MembershipId == user.MembershipId).DateLastPlayed = new DateTime(2017, 9, 6);
        }
        public void SaveUsers()
        {
            _storage.SaveUsers(users);
        }
        public List<User> GetAllUsers()
        {
            return new List<User>(users);
        }
        public List<User> GetGuildUsers(ulong guildId)
        {
            return new List<User>(users.Where(x => x.GuildIDs.Contains(guildId)).ToList());
        }
        public List<User> GetUsers(SocketCommandContext context)
        {
            ulong userId = context.Message.MentionedUsers.Count == 0 ? context.User.Id : context.Message.MentionedUsers.FirstOrDefault().Id;
            return new List<User>(users.Where(x => x.GuildIDs.Contains(context.Guild.Id)).Where(x => x.Completions.Count > 0).Where(x => x.DiscordID == userId).ToList());
        }
        public User GetUserByMembershipId(long membershipId)
        {
            return users.FirstOrDefault(x => x.MembershipId == membershipId);
        }
        public List<User> GetUsers(ulong guildId, ulong userId)
        {
            return new List<User>(users.Where(x => x.GuildIDs.Contains(guildId)).Where(x => x.DiscordID == userId).ToList());
        }
        public IEnumerable<User> GetUsersByDiscordId(ulong userId)
        {
            return users.Where(x => x.DiscordID == userId);
        }

        public void EditUser(User user)
        {
            users[users.IndexOf(users.FirstOrDefault(x => x.Guid == user.Guid))] = user;
            SaveUsers();
        }

        public User GetUserByGuid(Guid guid)
        {
            return users.FirstOrDefault(x => x.Guid == guid);
        }

        public async Task<UserResponse> CreateAndAddUser(ulong guildId, ulong discordID, RequestData requestData)
        {
            //if (users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault() != null) return new UserResponse() { User = users[guildID].Where(x => x.DiscordID == discordID).FirstOrDefault(), Code = 2 };
            if (users.FirstOrDefault(x => x.SteamID == requestData.SteamID) != null && requestData.SteamID != 0) return new UserResponse() { User = users.FirstOrDefault(x => x.SteamID == requestData.SteamID), Code = 3 };
            if (users.FirstOrDefault(x => x.MembershipId == requestData.MembershipId) != null) return new UserResponse() { User = users.FirstOrDefault(x => x.MembershipId == requestData.MembershipId), Code = 4 };

            GetHistoricalStatsForAccount getHistoricalStatsForAccount = await _requestHandler.GetHistoricalStatsForAccount(requestData.MembershipType, requestData.MembershipId);
            if (getHistoricalStatsForAccount.ErrorCode != 1) return new UserResponse() { User = null, Code = 5 };

            List<Character> characters = new List<Character>();
            foreach (DestinyHistoricalStatsPerCharacter character in getHistoricalStatsForAccount.Response.Characters)
            {
                characters.Add(new Character()
                {
                    CharacterId = character.CharacterId,
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
