using CodeHollow.FeedReader;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DragonLore.Models.Matches;
using System;
using System.Threading.Tasks;

namespace DragonLore.Managers
{
  public class BotMessageManager : IBotMessageManager
  {
    public async Task DirectMessageUserAsync(string message, SocketUser user, Embed embed)
    {
      var dMChannel = await user.GetOrCreateDMChannelAsync();
      await dMChannel.SendMessageAsync(message, embed: embed);
    }

    public async Task DirectMessageUserEmbedAsync(string messageContent, SocketUser user)
    {
      var embed = GenerateEmbed(messageContent);
      await user.SendMessageAsync("", embed: embed);
    }

    public async Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel)
    {
      return await channel.SendMessageAsync(text);
    }

    public async Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel, Embed embed)
    {
      return await channel.SendMessageAsync(text, embed: embed);
    }

    public async Task RemoveCommandMessageAsync(SocketMessage message, ISocketMessageChannel channel)
    {
      // Wait 5 seconds before deleting
      await Task.Delay(5000);
      await message.DeleteAsync();
    }

    public async Task RemoveCommandAndBotMessageAsync(SocketMessage message, ISocketMessageChannel channel, RestUserMessage botmessage)
    {
      // Wait 5 seconds before deleting
      await Task.Delay(5000);
      await message.DeleteAsync();
      await botmessage.DeleteAsync();
    }

    public async Task SendAndRemoveEmbedAsync(string messageContent, SocketCommandContext Context)
    {
      var embed = GenerateEmbed(messageContent);
      var message = await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandAndBotMessageAsync(Context.Message, Context.Channel, message);
    }

    public async Task SendAndRemoveEmbedAsync(string messageContent, SocketCommandContext Context, IGuildUser user)
    {
      var embed = GenerateEmbed(user, messageContent);
      var message = await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandAndBotMessageAsync(Context.Message, Context.Channel, message);
    }

    public async Task SendEmbedAndRemoveCommandAsync(string messageContent, SocketCommandContext Context, IGuildUser user)
    {
      var embed = GenerateEmbed(user, messageContent);
      await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandMessageAsync(Context.Message, Context.Channel);
    }

    public async Task SendNewsEmbedAsync(string source, FeedItem newsItem, ISocketMessageChannel channel)
    {
      var embed = GenerateNewsEmbed(newsItem, source);
      await DirectMessageChannelAsync("", channel, embed);
    }

    public Embed GenerateEmbed(string message)
    {
      var embed = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithDescription(message);
      return embed;
    }

    public Embed GenerateEmbed(IGuildUser user, string message)
    {
      var embed = new EmbedBuilder()
               .WithAuthor(user)
               .WithColor(Color.Gold)
               .WithDescription(message);
      return embed;
    }

    public Embed GenerateNewsEmbed(FeedItem newsItem, string source)
    {
      var embed = new EmbedBuilder()
               .WithColor(Color.Gold)
               .WithAuthor(source)
               .WithTitle(newsItem.Title)
               .WithUrl(newsItem.Link)
               .WithFooter(newsItem.PublishingDateString);

      //check the source so we know what to do with the Content of the News
      switch (source)
      {
        case "GosuGamers News":
          if (newsItem.Content != null)
          {
            int startPos = newsItem.Description.IndexOf("<p>") + "<p>".Length;
            int length = newsItem.Description.IndexOf("</p><p>") - startPos;
            embed.WithDescription(newsItem.Description.Substring(startPos, length));
          }
          break;

        case "HLTV News":
          embed.WithDescription("Click the title to find out more.");
          break;

        case "Counter-strike.net":
          var description = newsItem.Description.Replace("[&#8230;]", "");
          description = description.Replace("&#8211;", "\n-");
          if (description.Length > 1700)
          {
            description = newsItem.Description.Substring(0, 1700);
            description += $"{Environment.NewLine}Press the title to see the full patchnotes";
          }
          embed.WithDescription(description);
          break;
      }

      return embed;
    }

    public Embed GenerateUpcomingMatchesEmbed(Day matchDay) {

      var embed = new EmbedBuilder()
        .WithTitle(matchDay.Headline)
        .WithColor(Color.Gold);

      foreach (Match match in matchDay.Matches)
        embed.AddField(match.TeamA.Name + " vs " + match.TeamB.Name, match);

      return embed;
    }
  }
}