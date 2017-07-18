using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace DragonLore.Handlers
{
  public interface IBotMessageManager
  {
    Task DirectMessageUserAsync(string message, SocketUser user, Embed embed);

    Task DirectMessageUserEmbedAsync(string messageContent, IGuildUser user);

    Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel);

    Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel, Embed embed = null);

    Task RemoveCommandMessageAsync(SocketMessage message, ISocketMessageChannel channel);

    Task RemoveCommandAndBotMessageAsync(SocketMessage message, ISocketMessageChannel channel, RestUserMessage botmessage = null);

    Task SendAndRemoveEmbed(string messageContent, SocketCommandContext Context);

    Task SendAndRemoveEmbed(string messageContent, SocketCommandContext Context, IGuildUser user);

    Task SendEmbedAndRemoveCommand(string messageContent, SocketCommandContext Context, IGuildUser user);

    Task SendNewsEmbed(string source, SyndicationItem newsItem, ISocketMessageChannel channel);

    Embed GenerateEmbedAsync(string message);

    Embed GenerateEmbedAsync(IGuildUser user, string message);

    Embed GenerateNewsEmbedAsync(SyndicationItem newsItem, string source);
  }
}
