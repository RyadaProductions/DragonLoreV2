using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.Managers;
using DragonLore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DragonLore.Main
{
  public class CommandChecker
  {
    private readonly IChannels _channels;
    private readonly Settings _settings;
    private readonly IBotMessageManager _botMessage;
    private readonly LogManager _logManager;
    private readonly CommandService _commands;

    private readonly IServiceProvider _map;

    public CommandChecker(IServiceProvider map)
    {
      _map = map;

      _settings = map.GetService<Settings>();
      _commands = map.GetService<CommandService>();

      _botMessage = map.GetService<IBotMessageManager>();
      _logManager = map.GetService<LogManager>();

      _channels = map.GetService<IChannels>();
    }

    public async Task Spam(SocketUserMessage message)
    {
      if (message.Content.Contains("discord.gg/"))
      {
        var log = _settings.Client.GetChannel(_channels.AdminChannel) as SocketTextChannel;
        await _botMessage.DirectMessageChannelAsync(message.Author.Mention + " Advertised: ```" + message.Content + "```", log);

        await _logManager.Logger(new LogMessage(LogSeverity.Warning, "Spam", $"user: {message.Author} advertised: {message.Content}"));

        await message.DeleteAsync();
        return;
      }
    }

    public async Task Command(SocketUserMessage message)
    {
      int pos = 0;
      if (message.HasCharPrefix('!', ref pos))
      {
        await _logManager.Logger(new LogMessage(LogSeverity.Info, "Command", $"user: {message.Author} command: {message.Content}"));
        var context = new SocketCommandContext(_settings.Client, message);

        var result = await _commands.ExecuteAsync(context, pos, _map);

        if (!result.IsSuccess)
        {
          await _botMessage.SendAndRemoveEmbed(result.ErrorReason, context);
          await _logManager.Logger(new LogMessage(LogSeverity.Warning, "Command", $"user: {message.Author} command: {message.Content} error: {result.ErrorReason}"));
        }
      }
    }
  }
}