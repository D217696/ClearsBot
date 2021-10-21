using ClearsBot.Objects;
using System;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IRaids
    {
        Func<Completion, bool> GetCriteriaByRaid(Raid raid);
        Raid GetRaid(ulong guildId, string raidString);
        IEnumerable<Raid> GetRaids(ulong guildId);
        Raid SetRaidTime(ulong guildId, string raidString, TimeSpan time);
        Raid AddShortcut(ulong guildId, string raidString, string shortcut);
        void GuildJoined(ulong guildId);
    }
}