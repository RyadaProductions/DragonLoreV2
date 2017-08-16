namespace DragonLore.Models.Matches
{
    public class Match
    {
        public string Time { get; set; }

        public string Placeholder { get; set; }

        public string Name { get; set; }
        public string Logo { get; set; }

        public Team TeamA { get; set; }
        public Team TeamB { get; set; }

        public Match(string time)
        {
            Time = time;
            TeamA = TeamB = new Team() { Name = "TBA" };
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Placeholder))
                return Time + " " + Name;

            return Placeholder;
        }
    }
}