using AngleSharp;
using AngleSharp.Dom;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.Models.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DragonLore.UpcomingMatches {
  class HltvParser {
    private Timer _timer;
    private IBotMessageManager _botMessage;
    private ISocketMessageChannel _channel;
    
    public HltvParser(Settings settings, IChannels channels, IBotMessageManager botMessage) {
      _botMessage = botMessage;
      _channel = settings.Client.GetChannel(channels.UpcomingMatchesChannel) as ISocketMessageChannel;

      _timer = new Timer(async (e) => { await GetNewUpcomingMatches(); }, null, 0, 86400000);
    }
    
    private async Task GetNewUpcomingMatches() {
      var page = await GetCompletePage("https://www.hltv.org/matches");
      var currentlyUpcomming = ParseDocument(page);

      currentlyUpcomming.Reverse();

      foreach (Day matchDay in currentlyUpcomming) {
        var embed = _botMessage.GenerateUpcomingMatchesEmbed(matchDay);
        await _botMessage.DirectMessageChannelAsync("", _channel, embed);
      }
    }
    
    private async Task<IDocument> GetCompletePage(string address) {
      var config = Configuration.Default.WithDefaultLoader();
      return await BrowsingContext.New(config).OpenAsync(address);
    }
    
    private List<Day> ParseDocument(IDocument document) {
      var result = new List<Day>();

      // This CSS selector gets the desired content
      var matchDaySelector = ".upcoming-matches .match-day";
      var matchDays = document.QuerySelectorAll(matchDaySelector);

      foreach (var dayElem in matchDays) {
        var headline = dayElem.QuerySelector(".standard-headline");

        var matchDay = new Day(headline.TextContent);

        var matchElements = dayElem.QuerySelectorAll("a");
        foreach (var matchElement in matchElements) {
          var timeElem = matchElement.QuerySelector("div.time");

          var match = new Match(timeElem.TextContent);

          var placeHolder = matchElement.QuerySelector(".placeholder-text-cell");
          if (placeHolder != null) {
            match.Placeholder = placeHolder.TextContent;
          }

          var teamElems = matchElement.QuerySelectorAll(".team-cell");
          if (teamElems.Count() > 0) {
            match.TeamA = ParseTeamElement(teamElems[0]);
          }
          if (teamElems.Count() > 1) {
            match.TeamB = ParseTeamElement(teamElems[1]);
          }

          var eventNameElem = matchElement.QuerySelector(".event-name");
          if (eventNameElem != null) {
            match.Name = eventNameElem.TextContent;
          }

          var eventLogoElem = matchElement.QuerySelector(".event-logo");
          if (eventLogoElem != null) {
            match.Logo = eventLogoElem.GetAttribute("src");
          }

          matchDay.Matches.Add(match);
        }

        // Add to result
        result.Add(matchDay);
      }

      return result;
    }

    private Team ParseTeamElement(IElement element) {
      var result = new Team();

      var imgElem = element.QuerySelector("img");
      result.Logo = imgElem.GetAttribute("src");

      var nameElem = element.QuerySelector(".team");
      result.Name = nameElem.TextContent;

      return result;
    }
  }
}
