using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Objects
{
    public class ButtonData
    {
        public Guid InteractionID { get; set; }
        public string CommandName { get; set; }
        public ulong DiscordUserId { get; set; }
        public ulong DiscordServerId { get; set; }
        public ulong DiscordChannelId { get; set; }
        public long MembershipId { get; set; }
        public int MembershipType { get; set; }
        public Raid Raid { get; set; } = null;
        public int Page { get; set; } = 0;
        public bool Handled { get; set; }
        public bool PrivateButton { get; set; } = true;
    }
}
