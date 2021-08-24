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
    public class Guilds
    {
        public static Dictionary<ulong, Guild> guilds = new Dictionary<ulong, Guild>();
        private const string configFolder = "Resources";
        private const string configFile = "guilds.json";
        public static async Task Initialize()
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(guilds, Formatting.Indented));
            }
            else
            {
                string guildsString = File.ReadAllText(configFolder + "/" + configFile);
                if (guildsString == "")
                {
                    guilds = new Dictionary<ulong, Guild>();
                }
                else
                {
                    guilds = JsonConvert.DeserializeObject<Dictionary<ulong, Guild>>(guildsString);
                }
            }

            foreach (SocketGuild guild in Program._client.Guilds)
            {
                if (guilds.ContainsKey(guild.Id)) continue;
                guilds.Add(guild.Id, new Guild() { GuildId = guild.Id });
            }

            SaveGuilds();
        }

        public static Permissions.PermissionLevels GetPermissionForUser(IGuildUser user)
        {
            List<ulong> roles = JsonConvert.DeserializeObject<List<ulong>>(JsonConvert.SerializeObject(user.RoleIds));
            Guild guild = guilds[user.Guild.Id];
            if (user.Id == guild.AdminUser) return Permissions.PermissionLevels.AdminUser;
            if (roles.Contains(guild.AdminRole)) return Permissions.PermissionLevels.AdminRole;
            foreach (ulong roleId in guild.ModRoles)
            {
                if (roles.Contains(roleId)) return Permissions.PermissionLevels.ModRole;
            }
            return Permissions.PermissionLevels.User;
        }

        public static void SaveGuilds()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(guilds, Formatting.Indented));
        }
    }
}
