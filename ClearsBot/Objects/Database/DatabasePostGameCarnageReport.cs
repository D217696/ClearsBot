using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Objects.Database
{
    public class DatabasePostGameCarnageReport
    {
        public long InstanceId { get; set; }
        public ulong RaidHash { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Period { get; set; }
        public int StartingPhaseIndex { get; set; }
        public double Kills { get; set; }
        public int PlayerCount { get; set; }
        public GetPostGameCarnageReport GetPostGameCarnageReport { get; set; }
    }
}
