using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dragon_Lore.Handlers
{
  public class BotMessageManager : IBotMessageManager
  {
    public async Task DirectMessageUserAsync(string message, SocketUser user, Embed embed)
    {
      var dMChannel = await user.GetOrCreateDMChannelAsync();
      await dMChannel.SendMessageAsync(message, embed: embed);
    }

    public async Task DirectMessageUserEmbedAsync(string messageContent, IGuildUser user)
    {
      var dMChannel = await user.GetOrCreateDMChannelAsync();
      var embed = GenerateEmbedAsync(messageContent);
      await dMChannel.SendMessageAsync("", embed: embed);
    }

    public async Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel)
    {
      return await channel.SendMessageAsync(text);
    }

    public async Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel, Embed embed = null)
    {
      return await channel.SendMessageAsync(text, embed: embed);
    }

    public async Task RemoveCommandMessageAsync(SocketMessage message, ISocketMessageChannel channel)
    {
      // Wait 2 seconds before deleting
      await Task.Delay(5000);
      await message.DeleteAsync();
    }

    public async Task RemoveCommandAndBotMessageAsync(SocketMessage message, ISocketMessageChannel channel, RestUserMessage botmessage = null)
    {
      // Wait 2 seconds before deleting
      await Task.Delay(5000);
      await message.DeleteAsync();

      if (botmessage != null) await botmessage.DeleteAsync();
    }

    public async Task SendAndRemoveEmbed(string messageContent, SocketCommandContext Context)
    {
      var embed = GenerateEmbedAsync(messageContent);
      var message = await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandAndBotMessageAsync(Context.Message, Context.Channel, message);
    }

    public async Task SendAndRemoveEmbed(string messageContent, SocketCommandContext Context, IGuildUser user)
    {
      var embed = GenerateEmbedAsync(user, messageContent);
      var message = await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandAndBotMessageAsync(Context.Message, Context.Channel, message);
    }

    public async Task SendEmbedAndRemoveCommand(string messageContent, SocketCommandContext Context, IGuildUser user)
    {
      var embed = GenerateEmbedAsync(user, messageContent);
      await DirectMessageChannelAsync("", Context.Channel, embed);
      await RemoveCommandMessageAsync(Context.Message, Context.Channel);
    }

    public async Task SendNewsEmbed(string source, SyndicationItem newsItem, ISocketMessageChannel channel)
    {
      var embed = GenerateNewsEmbedAsync(newsItem, source);
      await DirectMessageChannelAsync("", channel, embed);
    }

    public Embed GenerateEmbedAsync(string message)
    {
      Embed embed = new EmbedBuilder()
                .WithColor(new Color(9912378))
                .WithDescription(message);
      return embed;
    }

    public Embed GenerateEmbedAsync(IGuildUser user, string message)
    {
      Embed embed = new EmbedBuilder()
               .WithAuthor(new EmbedAuthorBuilder()
                .WithName(user.Username)
                .WithIconUrl(user.GetAvatarUrl()))
               .WithColor(new Color(9912378))
               .WithDescription(message);
      return embed;
    }

    public Embed GenerateNewsEmbedAsync(SyndicationItem newsItem, string source)
    {
      EmbedBuilder embed = new EmbedBuilder();
      embed.WithColor(new Color(9912378));
      embed.WithAuthor(new EmbedAuthorBuilder()
        .WithName(source));
      embed.WithTitle(newsItem.Title.Text);
      embed.WithUrl(newsItem.Links[0].Uri.AbsoluteUri);
      embed.WithFooter(new EmbedFooterBuilder()
        .WithText(newsItem.PublishDate.DateTime.ToString()));
      //check the source so we know what to do with the summary of the News
      switch (source)
      {
        case "GosuGamers News":
          if (newsItem.Summary != null)
          {
            int startPos = newsItem.Summary.Text.IndexOf("<p>") + "<p>".Length;
            int length = newsItem.Summary.Text.IndexOf("</p><p>") - startPos;
            embed.WithDescription(newsItem.Summary.Text.Substring(startPos, length));
          }
          break;
        case "HLTV News":
          embed.WithDescription("Click the title to find out more.");
          break;
        case "Counter-strike.net":
          embed.WithDescription(newsItem.Summary.Text.Replace("&#8211;", "\n-"));
          if (newsItem.Summary.Text.Length > 1700)
          {
            var limited = newsItem.Summary.Text.Substring(0, 1700);
            limited += "\n Press the title to see the full patchnotes";
          }
          break;
      }

      return embed;
    }
  }
}
