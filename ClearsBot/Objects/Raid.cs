using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class Raid
    {
        public string DisplayName { get; set; }
        public TimeSpan CompletionTime { get; set; }
        public List<string> Shortcuts { get; set; } = new List<string>();
        public List<ulong> Hashes { get; set; } = new List<ulong>();
        public Color Color { get; set; }
        public int StartingPhaseIndexToBeFresh { get; set; } = 0;
        public ulong FirstRole { get; set; }
        public ulong SecondRole { get; set; }
        public ulong ThirdRole { get; set; }
    }
}
