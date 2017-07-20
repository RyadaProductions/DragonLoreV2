using CodeHollow.FeedReader;
using Discord;
using DragonLore.Managers;
using DragonLore.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DragonLore.Rss
{
  internal class RssReader : IRssReader
  {
    private readonly RssService _service;
    private readonly LogManager _logManager;

    private readonly Timer _rssTimer;

    private string _rss = "gosu";

    public RssReader(RssService service, LogManager logmanager)
    {
      _service = service;
      _logManager = logmanager;

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

    public async Task NewsRSSAsync(string source, string url)
    {
      try
      {
        var feed = await FeedReader.ReadAsync(url);

        var news = feed.Items.First();

        await _service.CheckNewRss(_rss, source, news);
      }
      catch
      {
        await _logManager.Logger(new LogMessage(LogSeverity.Warning, "RssReader", $"could not parse the { source } feed"));
      }
    }
  }
}