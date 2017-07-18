using System.Collections.Generic;

namespace DragonLore.Models
{
  public class Serializing
  {
    public List<string> Servers { get; set; } = new List<string>();

    public bool WelcomeBool { get; set; }
    public string WelcomeMessage { get; set; }

    public string LastGosuRss { get; set; }
    public string LastHltvRss { get; set; }
    public string LastValveRss { get; set; }

  }
}
