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
                CharacterCount = x.Characters.Count(),
                CompletionsCount = x.Characters.Count(),
                DateRegistered = x.DateRegistered.ToShortDateString()
            });
            return users.ToList();
        }
    }

    public class SimplifiedUser
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public string DiscordId { get; set; }
        public string MembershipId { get; set; }
        public int MembershipType { get; set; }
        public int CharacterCount { get; set; }
        public int CompletionsCount { get; set; }
        public string DateRegistered { get; set; }
    }
}
