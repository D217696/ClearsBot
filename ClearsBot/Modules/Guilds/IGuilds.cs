using ClearsBot.Objects;
using Discord.WebSocket;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IGuilds
    {
        void GuildJoined(ulong guildId, ulong guildOwnerId);
        void SaveGuilds();
        public Dictionary<ulong, Guild> GetGuilds();
        public Guild GetGuild(ulong guildId);
        public SocketGuild GetGuildFromClient(ulong guildId);
    }
}