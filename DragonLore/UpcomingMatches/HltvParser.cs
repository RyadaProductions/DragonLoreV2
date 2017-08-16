using System;
using AngleSharp;
using AngleSharp.Dom;
using Discord;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.Models.Matches;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DragonLore.UpcomingMatches
{
    internal class HltvParser
    {
        private Timer _timer;
        private readonly IBotMessageManager _botMessage;
        private readonly ulong _channelId;
        private readonly DiscordSocketClient _client;

        public HltvParser(Settings settings, IChannels channels, IBotMessageManager botMessage)
        {
            _botMessage = botMessage;
            _client = settings.Client;
            _channelId = channels.UpcomingMatchesChannel;

            _timer = new Timer(async (e) => { await GetNewUpcomingMatches(); }, null, 0, 86400000);
        }

        private async Task GetNewUpcomingMatches()
        {
            try
            {
                var channel = _client.GetChannel(_channelId) as ISocketMessageChannel;

                var oldMessages = await channel.GetMessagesAsync().Flatten();
                await channel.DeleteMessagesAsync(oldMessages);

                var page = await GetCompletePage("https://www.hltv.org/matches");
                var currentlyUpcomming = ParseDocument(page);

                currentlyUpcomming.Reverse();

                foreach (Day matchDay in currentlyUpcomming)
                {
                    var embed = _botMessage.GenerateUpcomingMatchesEmbed(matchDay);
                    await _botMessage.DirectMessageChannelAsync("", channel, embed);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ignorable error in HltvParser " + Environment.NewLine + e);
            }
        }

        private async Task<IDocument> GetCompletePage(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            return await BrowsingContext.New(config).OpenAsync(address);
        }

        private List<Day> ParseDocument(IDocument document)
        {
            var result = new List<Day>();

            // This CSS selector gets the desired content
            var matchDaySelector = ".upcoming-matches .match-day";
            var matchDays = document.QuerySelectorAll(matchDaySelector);

            foreach (var dayElem in matchDays)
            {
                var headline = dayElem.QuerySelector(".standard-headline");

                var matchDay = new Day(headline.TextContent);

                var matchElements = dayElem.QuerySelectorAll("a");
                foreach (var matchElement in matchElements)
                {
                    var timeElem = matchElement.QuerySelector("div.time");

                    var match = new Match(timeElem.TextContent);

                    var placeHolder = matchElement.QuerySelector(".placeholder-text-cell");
                    if (placeHolder != null)
                    {
                        match.Placeholder = placeHolder.TextContent;
                    }

                    var teamElems = matchElement.QuerySelectorAll(".team-cell");
                    if (teamElems.Any())
                    {
                        match.TeamA = ParseTeamElement(teamElems[0]);
                    }
                    if (teamElems.Count() > 1)
                    {
                        match.TeamB = ParseTeamElement(teamElems[1]);
                    }

                    var eventNameElem = matchElement.QuerySelector(".event-name");
                    if (eventNameElem != null)
                    {
                        match.Name = eventNameElem.TextContent;
                    }

                    var eventLogoElem = matchElement.QuerySelector(".event-logo");
                    if (eventLogoElem != null)
                    {
                        match.Logo = eventLogoElem.GetAttribute("src");
                    }

                    matchDay.Matches.Add(match);
                }

                // Add to result
                result.Add(matchDay);
            }

            return result;
        }

        private Team ParseTeamElement(IElement element)
        {
            var result = new Team();

            var imgElem = element.QuerySelector("img");
            try
            {
                result.Logo = imgElem.GetAttribute("src");
            }
            catch { }

            var nameElem = element.QuerySelector(".team");
            result.Name = nameElem.TextContent;

            return result;
        }
    }
}