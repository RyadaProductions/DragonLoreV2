using Discord.Audio;
using Discord.WebSocket;
using System.Collections.Generic;

namespace DragonLore.Models
{
    public class Settings
    {
        public readonly DiscordSocketClient Client;

        public IEnumerable<SocketRole> Ranks { get; set; } = new List<SocketRole>();
        public IEnumerable<string> Servers { get; set; } = new List<string>();

        public bool IsWelcomeMessageOn { get; set; }
        public string WelcomeMessage { get; set; }

        public string CurrentSong { get; set; }
        public IAudioClient VoiceClient { get; set; }

        public Dictionary<string, string> LastRss { get; set; } = new Dictionary<string, string>()
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