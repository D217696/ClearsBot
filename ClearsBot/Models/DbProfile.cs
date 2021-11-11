using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Models
{
    public class DbProfile
    {
        public long Membership_id { get; set; }
        public int User_id { get; set; }
        public int Membership_type { get; set; }
        public long Steam_Id { get; set; }
        public DateTime Date_last_played { get; set; }
        public DateTime Date_registered { get; set; }
    }
}
