using ClearsBot.Objects;
using Discord.WebSocket;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IGuilds
    {
        void JoinedGuild(ulong guildId, ulong guildOwnerId);
        void SaveGuilds();
        void SyncGuilds();
        public Dictionary<ulong, InternalGuild> GetGuilds();
        public SocketGuild GetGuildFromClient(ulong guildId);
        public InternalGuild GetGuild(ulong guildId);
        public void LeftGuild(ulong guildId);
        public string GetGuildLanguage(ulong guildId);
    }
}