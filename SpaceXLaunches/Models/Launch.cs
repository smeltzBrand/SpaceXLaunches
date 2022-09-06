using System;

namespace SpaceXLaunches.Models
{
    public class Launch
    {
        public string Id { get; set; }
        public DateTime LaunchDate { get; set; }
        public string LaunchTime { get; set; }
        public string RocketName { get; set; }
        public string LaunchStatus { get; set; }
        public float PayloadMass { get; set; }
        public int PayloadRank { get; set; }

    }
}
