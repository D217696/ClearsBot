using ClearsBot.Objects;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IGuilds
    {
        Dictionary<ulong, Guild> GuildsList { get; set; }
        void GuildJoined(ulong guildId, ulong guildOwnerId);
        void SaveGuilds();
    }
}