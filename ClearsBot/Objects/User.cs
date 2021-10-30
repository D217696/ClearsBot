using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GroupedUser
    {
        public ulong DiscordID { get; set; } = 0;
        public List<User> Users { get; set; } = new List<User>();
    }

    public class User
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public ulong DiscordID { get; set; }
        public ulong GuildID { get; set; } = 0;
        public long SteamID { get; set; }
        public long MembershipId { get; set; }
        public int MembershipType { get; set; }
        public DateTime DateLastPlayed { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();
        public Dictionary<long, Completion> Completions { get; set; } = new Dictionary<long, Completion>();
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
        public List<ulong> GuildIDs { get; set; } = new List<ulong>();
    }

    public class Completion
    {
        public long InstanceID { get; set; }
        public ulong RaidHash { get; set; }
        public double Kills { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Period { get; set; }
        public int StartingPhaseIndex { get; set; }
    }
    public class Character
    {
        public long CharacterId { get; set; }
        public bool Deleted { get; set; }
        public bool Handled { get; set; }
    }
}
