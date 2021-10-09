using ClearsBot.Objects;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Guilds : IGuilds
    {
        readonly ILogger _logger;
        private Dictionary<ulong, Guild> GuildsList { get; set; } = new Dictionary<ulong, Guild>();
        private string ConfigFolder { get; set; } = "Resources";
        private string ConfigFile { get; set; } = "guilds.json";
        public Guilds(ILogger logger)
        {
            _logger = logger;

            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

            if (!File.Exists(ConfigFolder + "/" + ConfigFile))
            {
                File.WriteAllText(ConfigFolder + "/" + ConfigFile, JsonConvert.SerializeObject(GuildsList, Formatting.Indented));
            }
            else
            {
                string guildsString = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
                if (guildsString == "")
                {
                    GuildsList = new Dictionary<ulong, Guild>();
                }
                else
                {
                    GuildsList = JsonConvert.DeserializeObject<Dictionary<ulong, Guild>>(guildsString);
                }
            }

            foreach (SocketGuild guild in Program._client.Guilds)
            {
                if (GuildsList.ContainsKey(guild.Id)) continue;
                GuildsList.Add(guild.Id, new Guild() { GuildId = guild.Id });
            }
            SaveGuilds();

            Program._client.JoinedGuild += _client_JoinedGuild;
        }
        private Task _client_JoinedGuild(SocketGuild arg)
        {
            return Task.Run(() => GuildJoined(arg.Id, arg.OwnerId));
        }

        public void GuildJoined(ulong guildId, ulong guildOwnerId)
        {
            GuildsList.Add(guildId, new Guild()
            {
                AdminUser = guildOwnerId,
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
            File.WriteAllText(ConfigFolder + "/" + ConfigFile, JsonConvert.SerializeObject(GuildsList, Formatting.Indented));
        }
    }
}
