using ClearsBot.Objects;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Guilds : IGuilds
    {
        readonly ILogger _logger;
        readonly IStorage _storage;
        private Dictionary<ulong, Guild> GuildsList { get; set; } = new Dictionary<ulong, Guild>();
        public Guilds(ILogger logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;

            GuildsList = _storage.GetGuildsFromStorage();

            foreach (SocketGuild guild in Program._client.Guilds)
            {
                if (GuildsList.ContainsKey(guild.Id)) continue;
                GuildsList.Add(guild.Id, new Guild() { GuildId = guild.Id });
            }
            SaveGuilds();
        }

        public void GuildJoined(ulong guildId, ulong guildOwnerId)
        {
            if (GuildsList.ContainsKey(guildId)) return;
            GuildsList.Add(guildId, new Guild()
            {
                GuildOwner = guildOwnerId,
                GuildId = guildId
            });
            SaveGuilds();
        }
        public Dictionary<ulong, Guild> GetGuilds()
        {
            return GuildsList;
        }
        public Guild GetGuild(ulong guildId)
        {
            if (GuildsList.ContainsKey(guildId)) return GuildsList[guildId];
            _logger.LogError($"Tried to get guild: {guildId}, could not get from list");
            return null;
        }
        public SocketGuild GetGuildFromClient(ulong guildId)
        {
            if (Program._client.Guilds.FirstOrDefault(x => x.Id == guildId) != null) return Program._client.Guilds.FirstOrDefault(x => x.Id == guildId);
            _logger.LogError($"Tried to get guild: {guildId}, could not get from client");
            return null;
        }
        public void SaveGuilds()
        {
            _storage.SaveGuilds(GuildsList);
        }
    }
}
