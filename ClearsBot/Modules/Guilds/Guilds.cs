using ClearsBot.Objects;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Guilds : IGuilds
    {
        public Dictionary<ulong, Guild> GuildsList { get; set; } = new Dictionary<ulong, Guild>();
        private string ConfigFolder { get; set; } = "Resources";
        private string ConfigFile { get; set; } = "guilds.json";
        public Guilds()
        {
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

        public void SaveGuilds()
        {
            File.WriteAllText(ConfigFolder + "/" + ConfigFile, JsonConvert.SerializeObject(GuildsList, Formatting.Indented));
        }
    }
}
