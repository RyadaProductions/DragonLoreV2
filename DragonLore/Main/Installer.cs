﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DragonLore.Models;
using DragonLore.Managers;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using DragonLore.MagicNumbers.Roles;
using DragonLore.MagicNumbers.Channels;
using DragonLore.Services;

namespace DragonLore.Main
{
  internal class Installer
  {
    private readonly DiscordSocketClient _client;

    private readonly CommandService _commands;
    private readonly LogManager _logManager;
    private readonly Settings _settings;

    private IServiceProvider _map;

    public Installer(DiscordSocketClient client)
    {
      _client = client;

      _commands = new CommandService();
      _logManager = new LogManager();
      _settings = new Settings(_client);
    }

    public async Task ModifyStatus()
    {
      await _client.SetStatusAsync(UserStatus.DoNotDisturb);
      await _client.SetGameAsync("with Ryada");
    }

    public async Task InstallCommands()
    {
      IServiceCollection map = new ServiceCollection();

      map.AddSingleton(_settings);
      map.AddSingleton(_commands);
      map.AddSingleton(_logManager);

      map.AddTransient<SaveLoadService, SaveLoadService>();
      map.AddTransient<RssService, RssService>();

      map.AddTransient<LogManager, LogManager>();
      map.AddTransient<IBotMessageManager, BotMessageManager>();

      // Magic number classes change Live to Test and it will change the ID's to supplement the test server
      map.AddTransient<IRoles, TestRoles>();
      map.AddTransient<IChannels, TestChannels>();

      _map = map.BuildServiceProvider();

      await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
    }

    public void InstallEvents()
    {
      var events = new Events(_map);

      _client.Log += _logManager.Logger;

      _client.MessageReceived += events.CmdHandler;
      _client.UserJoined += events.WelcomeHandler;
      _client.UserVoiceStateUpdated += events.UserVoiceStateUpdated;
      _client.Connected += events.Connected;
    }
  }
}