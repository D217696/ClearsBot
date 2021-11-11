using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Models
{
    public class DbCharacter
    {
        public long Character_id { get; set; }
        public long Membership_id { get; set; }
        public bool Deleted { get; set; }
        public bool Handled { get; set; }
    }
}
