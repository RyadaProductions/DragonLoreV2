﻿using CodeHollow.FeedReader;
using DragonLore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DragonLore.Rss
{
  internal class RssReader : IRssReader
  {
    private readonly RssService _service;

    private readonly Timer _rssTimer;

    private string _rss = "gosu";

    public RssReader(RssService service)
    {
      _service = service;

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
      catch (Exception ex)
      {
        Console.WriteLine(source + ex);
      }
    }
  }
}