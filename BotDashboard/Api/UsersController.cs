using ClearsBot.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotDashboard.Api
{
    [Route("api/[controller]", Name = nameof(UsersController))]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ClearsBot.Modules.Users _users;
        public UsersController(ClearsBot.Modules.Users users)
        {
            _users = users;
        }

        [HttpGet("GetUsers")]
        public async Task<List<SimplifiedUser>> GetUsers()
        {
            var users = _users.GetAllUsers().Select(x => new SimplifiedUser()
            {
                Guid = x.Guid,
                Username = x.Username,
                DiscordId = x.DiscordID.ToString(),
                MembershipId = x.MembershipId.ToString(),
                MembershipType = x.MembershipType,
                Characters = x.Characters.Select(x => new SimplifiedCharacter()
                {
                    CharacterId = x.CharacterId.ToString(),
                    Handled = x.Handled,
                    Deleted = x.Deleted
                }),
                CompletionsCount = x.Characters.Count(),
                DateRegistered = x.DateRegistered.ToShortDateString(),
                DateLastPlayed = x.DateLastPlayed.ToShortDateString()
            });
            return users.ToList();
        }

        [HttpPost("UpdateUser")]
        public async Task<Response> UpdateUser(SimplifiedUser user)
        {
            if (!ulong.TryParse(user.DiscordId, out ulong discordId)) return new Response() { code = 2, message = "Couldn't convert discord id to ulong" };
            if (!DateTime.TryParse(user.DateLastPlayed, out DateTime dateLastPlayed)) return new Response() { code = 3, message = "Couldn't convert date last played to datetime" };

            List<Character> characters = new List<Character>();
            foreach (SimplifiedCharacter character in user.Characters)
            {
                if (!long.TryParse(character.CharacterId, out long characterId)) return new Response() { code = 4, message = "Couldn't convert character id to long" };
                characters.Add(new Character()
                {
                    CharacterId = characterId,
                    Handled = character.Handled,
                    Deleted = character.Deleted
                });
            }

            User userFromList = _users.GetUserByGuid(user.Guid);
            _users.EditUser(new ClearsBot.Objects.User()
            {
                Guid = user.Guid,
                DiscordID = discordId,
                Username = user.Username,
                Characters = characters,
                DateRegistered = userFromList.DateRegistered,
                GuildID = userFromList.GuildID,
                GuildIDs = userFromList.GuildIDs,
                MembershipId = userFromList.MembershipId,
                MembershipType = userFromList.MembershipType,
                Completions = userFromList.Completions,
                SteamID = userFromList.SteamID,
                DateLastPlayed = dateLastPlayed,
            });

            return new Response() { code = 1, message = "Success!" };
        }
    }

    public class SimplifiedUser
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public string DiscordId { get; set; }
        public string MembershipId { get; set; }
        public int MembershipType { get; set; }
        public IEnumerable<SimplifiedCharacter> Characters { get; set; }
        public int CompletionsCount { get; set; }
        public string DateRegistered { get; set; }
        public string DateLastPlayed { get; set; }
    }
    public class SimplifiedCharacter
    {
        public string CharacterId { get; set; }
        public bool Handled { get; set; }
        public bool Deleted { get; set; }
    }
    public class Response
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
