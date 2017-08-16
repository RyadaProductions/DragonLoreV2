using CodeHollow.FeedReader;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DragonLore.Models.Matches;
using System.Threading.Tasks;

namespace DragonLore.Managers
{
    public interface IBotMessageManager
    {
        Task DirectMessageUserAsync(string message, SocketUser user, Embed embed);

        Task DirectMessageUserEmbedAsync(string messageContent, SocketUser user);

        Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel);

        Task<RestUserMessage> DirectMessageChannelAsync(string text, ISocketMessageChannel channel, Embed embed);

        Task RemoveCommandMessageAsync(SocketMessage message, ISocketMessageChannel channel);

        Task RemoveCommandAndBotMessageAsync(SocketMessage message, ISocketMessageChannel channel, RestUserMessage botmessage = null);

        Task SendAndRemoveEmbedAsync(string messageContent, SocketCommandContext context);

        Task SendAndRemoveEmbedAsync(string messageContent, SocketCommandContext context, IGuildUser user);

        Task SendEmbedAndRemoveCommandAsync(string messageContent, SocketCommandContext context, IGuildUser user);

        Task SendNewsEmbedAsync(string source, FeedItem newsItem, ISocketMessageChannel channel);

        Embed GenerateEmbed(string message);

        Embed GenerateEmbed(IGuildUser user, string message);

        Embed GenerateNewsEmbed(FeedItem newsItem, string source);

        Embed GenerateUpcomingMatchesEmbed(Day matchDay);
    }
}