using System.Collections.Generic;

namespace DragonLore.Models.Matches
{
    public class Day
    {
        public string Headline { get; set; }

        public List<Match> Matches { get; set; } = new List<Match>();

        public Day(string headline)
        {
            Headline = headline;
        }
    }
}