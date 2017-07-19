using Discord.Audio;
using Discord.WebSocket;
using System.Collections.Generic;

namespace DragonLore.Models
{
  public class Settings
  {
    //test server: 280625010337513475
    //live server: 278487570806210571
    public readonly DiscordSocketClient Client;

    public IEnumerable<SocketRole> Ranks { get; set; } = new List<SocketRole>();
    public IEnumerable<string> Servers { get; set; } = new List<string>();

    public bool IsWelcomeMessageOn { get; set; }
    public string WelcomeMessage { get; set; }

    public string CurrentSong { get; set; }
    public IAudioClient VoiceClient { get; set; }

    public Dictionary<string, string> LastRSS { get; set; } = new Dictionary<string, string>()
    {
      ["gosu"] = "",
      ["hltv"] = "",
      ["valve"] = ""
    };

    public Dictionary<string, string> Songs { get; set; } = new Dictionary<string, string>();

    public Settings(DiscordSocketClient client)
    {
      Client = client;
    }
  }
}