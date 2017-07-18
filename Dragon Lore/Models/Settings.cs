using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Models;

namespace DragonLore.Models
{
  public class Settings
  {
    //test server: 280625010337513475
    //live server: 278487570806210571
    public readonly DiscordSocketClient Client;

    public List<SocketRole> Ranks { get; } = new List<SocketRole>();
    public List<string> Servers { get; } = new List<string>();

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

    public Dictionary<string, VideoInfo> Songs { get; set; } = new Dictionary<string, VideoInfo>();

    public Settings(DiscordSocketClient client)
    {
      Client = client;
    }
  }
}
