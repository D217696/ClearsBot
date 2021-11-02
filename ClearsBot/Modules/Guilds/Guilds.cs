using ClearsBot.Modules;
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
        readonly DiscordSocketClient _client;
        private Dictionary<ulong, InternalGuild> GuildsList { get; set; } = new Dictionary<ulong, InternalGuild>();
        public Guilds(ILogger logger, IStorage storage, DiscordSocketClient client)
        {
            _logger = logger;
            _storage = storage;
            _client = client;

            GuildsList = _storage.GetGuildsFromStorage();

            foreach (SocketGuild guild in _client.Guilds)
            {
                if (GuildsList.ContainsKey(guild.Id)) continue;
                GuildsList.Add(guild.Id, new InternalGuild() { GuildId = guild.Id });
            }
            SaveGuilds();
        }

        public void JoinedGuild(ulong guildId, ulong guildOwnerId)
        {
            if (GuildsList.ContainsKey(guildId)) return;
            GuildsList.Add(guildId, new InternalGuild()
            {
                GuildOwner = guildOwnerId,
                GuildId = guildId
            });
            SaveGuilds();
        }
        public void SyncGuilds()
        {
            Dictionary<ulong, InternalGuild> guildsCopy = new Dictionary<ulong, InternalGuild>(GuildsList);
            foreach (InternalGuild guild in guildsCopy.Values)
            {
                if (_client.Guilds.FirstOrDefault(x => x.Id == guild.GuildId) != null)
                {
                    guild.IsActive = true;
                }
                else
                {
                    guild.IsActive = false;
                }
            }
            GuildsList = guildsCopy;

            foreach (SocketGuild guild in _client.Guilds.Where(x => !GuildsList.ContainsKey(x.Id)))
            {
                JoinedGuild(guild.Id, guild.OwnerId);
            }

            SaveGuilds();
        }

        public void LeftGuild(ulong guildId)
        {
            if (GuildsList.ContainsKey(guildId)) GuildsList[guildId].IsActive = false;
        }

        public bool EditGuildGlobalLeaderboards(ulong guildId)
        {
            if (GuildsList.ContainsKey(guildId))
            {
                GuildsList[guildId].GlobalLeaderboards = !GuildsList[guildId].GlobalLeaderboards;
            }

            return GuildsList[guildId].GlobalLeaderboards;
        }

        public string GetGuildLanguage(ulong guildId)
        {
            if (GuildsList.ContainsKey(guildId))
            {
                return GuildsList[guildId].Language;
            }

            return "en";
        }
        public Dictionary<ulong, InternalGuild> GetGuilds()
        {
            return GuildsList;
        }
        public InternalGuild GetGuild(ulong guildId)
        {
            if (GuildsList.ContainsKey(guildId)) return GuildsList[guildId];
            _logger.LogError($"Tried to get guild: {guildId}, could not get from list");
            return null;
        }

        public SocketGuild GetGuildFromClient(ulong guildId)
        {
            if (_client.Guilds.FirstOrDefault(x => x.Id == guildId) != null) return _client.Guilds.FirstOrDefault(x => x.Id == guildId);
            _logger.LogError($"Tried to get guild: {guildId}, could not get from client");
            return null;
        }
        public void SaveGuilds()
        {
            _storage.SaveGuilds(GuildsList);
        }
    }
}
