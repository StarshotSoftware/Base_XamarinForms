using System;
using SQLite.Net.Attributes;

namespace BaseStarShot.Model
{
    public class SuggestedRoute
    {
        public string DurationText { get; set; }
        public long DurationValue { get; set; }

        public string DistanceText { get; set; }

        public long DistanceValue { get; set; }
    }
}
