﻿using Discord;
using Discord.WebSocket;
using DragonLore.Handlers;
using DragonLore.MagicNumbers.Channels;
using DragonLore.MagicNumbers.Roles;
using DragonLore.Models;
using DragonLore.RSS;
using DragonLore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DragonLore.Main
{
  internal class Events
  {
    private readonly SaveLoadService _saveLoadService;
    private readonly Settings _settings;
    private readonly IServiceProvider _map;

    private readonly IBotMessageManager _botMessage;
    private readonly IRoles _roles;
    private readonly IChannels _channels;
    private readonly CommandChecker _commandChecker;

    public Events(IServiceProvider map)
    {
      _map = map;

      _settings = map.GetService<Settings>();
      _saveLoadService = map.GetService<SaveLoadService>();
      _botMessage = map.GetService<IBotMessageManager>();
      _roles = map.GetService<IRoles>();
      _channels = map.GetService<IChannels>();

      _commandChecker = new CommandChecker(map);
    }

    public async Task Connected()
    {
      IRssReader mainRSS = new RssReader(_map);
      if (_saveLoadService.LoadVars())
        Console.WriteLine("Settings loaded succesfully.");
      else
        Console.WriteLine("Error loading Settings, this is normal if it is a fresh installation.");
      _settings.Client.Connected -= Connected;
    }

    public async Task WelcomeHandler(SocketGuildUser arg)
    {
      var user = arg as IGuildUser;
      if (user == null) return;

      await user.AddRoleAsync(user.Guild.GetRole(_roles.Unranked));

      if (!_settings.IsWelcomeMessageOn || _settings.WelcomeMessage == "") return;
      await _botMessage.DirectMessageUserEmbedAsync(_settings.WelcomeMessage, user);
    }

    public async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState voiceBefore, SocketVoiceState voiceAfter)
    {
      var guildUser = user as SocketGuildUser;
      if (voiceAfter.VoiceChannel.Id == _channels.MusicChannel) await guildUser.AddRoleAsync(guildUser.Guild.GetRole(_roles.Music));
      else await guildUser.RemoveRoleAsync(guildUser.Guild.GetRole(_roles.Music));
    }

    public async Task CmdHandler(SocketMessage arg)
    {
      var msg = arg as SocketUserMessage;
      if (msg == null || msg.Author.IsBot) return;

      await _commandChecker.Spam(msg);
      await _commandChecker.Command(msg);
    }
  }
}