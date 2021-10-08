using ClearsBot.Objects;
using System;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IRaids
    {
        Func<Completion, bool> GetCriteriaByRaid(Raid raid);
        Raid GetRaid(ulong guildId, string raidString);
        List<Raid> GetRaids(ulong guildId);
        void SaveRaids();
    }
}