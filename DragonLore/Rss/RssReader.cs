using System;
using CodeHollow.FeedReader;
using Discord;
using DragonLore.Managers;
using DragonLore.Services;
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

            _rssTimer = new Timer(async (e) => { await RssTimerCallback(); }, null, 0, 5000);
        }

        private async Task RssTimerCallback()
        {
            switch (_rss)
            {
                case "gosu":
                    await NewsRssAsync("GosuGamers News", "http://www.gosugamers.net/counterstrike/news/rss");
                    _rss = "hltv";
                    break;

                case "hltv":
                    await NewsRssAsync("HLTV News", "http://www.hltv.org/news.rss.php");
                    _rss = "valve";
                    break;

                case "valve":
                    await NewsRssAsync("Counter-strike.net", "http://blog.counter-strike.net/index.php/feed/");
                    _rss = "gosu";
                    break;
            }
        }

        public async Task NewsRssAsync(string source, string url)
        {
            try
            {
                var feed = await FeedReader.ReadAsync(url);

                var news = feed.Items.First();

                await _service.CheckNewRss(_rss, source, news);
            }
            catch (Exception e)
            {
                await _logManager.Logger(new LogMessage(LogSeverity.Warning, "RssReader", $"could not parse the { source } feed {Environment.NewLine + e}"));
            }
        }
    }
}