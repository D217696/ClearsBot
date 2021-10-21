using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Objects
{
    public class Guild
    {
        public ulong GuildId { get; set; } = 0;
        public string Prefix { get; set; } = "$";
        public string Language { get; set; } = "en";
        public ulong FirstRole { get; set; } = 0;
        public ulong SecondRole { get; set; } = 0;
        public ulong ThirdRole { get; set; } = 0;
        public ulong GuildOwner { get; set; } = 0;
        public ulong AdminRole { get; set; } = 0;
        public List<ulong> ModRoles { get; set; } = new List<ulong>();
        public List<Milestone> Milestones { get; set; } = new List<Milestone>();
    }
    public class Milestone
    {
        public int Completions { get; set; } = 0;
        public ulong Role { get; set; } = 0;
        public Raid Raid { get; set; } = null;
    }
}
