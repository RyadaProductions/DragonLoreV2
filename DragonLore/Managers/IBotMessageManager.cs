using CodeHollow.FeedReader;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DragonLore.Managers
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

    Task SendNewsEmbed(string source, FeedItem newsItem, ISocketMessageChannel channel);

    Embed GenerateEmbedAsync(string message);

    Embed GenerateEmbedAsync(IGuildUser user, string message);

    Embed GenerateNewsEmbedAsync(FeedItem newsItem, string source);
  }
}
