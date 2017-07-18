using Discord.WebSocket;
using Dragon_Lore.Handlers;
using Dragon_Lore.MagicNumbers.Channels;
using Dragon_Lore.Services;
using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Dragon_Lore.Models;

namespace Dragon_Lore.RSS
{
  class MainRSS : IRSS
  {
    private readonly Settings _settings;
    private readonly SaveLoadService _saveLoadService;
    private readonly IBotMessageManager _botMessage;
    private readonly IChannels _channels;

    private readonly Timer _rssTimer;

    private string _rss = "gosu";

    public MainRSS(IServiceProvider map)
    {
      _settings = map.GetService<Settings>();
      _saveLoadService = map.GetService<SaveLoadService>();

      _botMessage = map.GetService<BotMessageManager>();
      _channels = map.GetService<IChannels>();

      _rssTimer = new Timer(async (e) => { await RSSTimerCallback(); }, null, 0, 5000);
    }

    private async Task RSSTimerCallback()
    {
      switch (_rss)
      {
        case "gosu":
          await NewsRSSAsync("GosuGamers News", "http://www.gosugamers.net/counterstrike/news/rss");
          _rss = "hltv";
          break;
        case "hltv":
          await NewsRSSAsync("HLTV News", "http://www.hltv.org/news.rss.php");
          _rss = "valve";
          break;
        case "valve":
          await NewsRSSAsync("Counter-strike.net", "http://blog.counter-strike.net/index.php/feed/");
          _rss = "gosu";
          break;
      }
    }

    public async Task NewsRSSAsync(string name, string url)
    {
      try
      {
        Rss20FeedFormatter rssFormatter;

        using (var xmlReader = XmlReader.Create(url))
        {
          rssFormatter = new Rss20FeedFormatter();
          rssFormatter.ReadFrom(xmlReader);
        }

        var channel = _settings.Client.GetChannel(_channels.NewsChannel) as ISocketMessageChannel;

        string newest = rssFormatter.Feed.Items.First().Id.ToString();

        if (_settings.LastRSS[_rss] != newest)
        {
          await _botMessage.SendNewsEmbed(name, rssFormatter.Feed.Items.First(), channel);
          _settings.LastRSS[_rss] = newest;
          _saveLoadService.SaveVars();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(name + ex);
      }
    }
  }
}
