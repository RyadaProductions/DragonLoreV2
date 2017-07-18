using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dragon_Lore.Handlers;
using Dragon_Lore.MagicNumbers.Channels;
using Dragon_Lore.MagicNumbers.Roles;
using Dragon_Lore.Models;
using Dragon_Lore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dragon_Lore.Main
{
  class Installer
  {
    private readonly DiscordSocketClient _client;

    private readonly CommandService _commands;
    private readonly LogManager _logManager;
    private readonly Settings _settings;

    private IServiceProvider _map;


    public Installer(DiscordSocketClient client)
    {
      _client = client ?? throw new NullReferenceException(nameof(DiscordSocketClient));

      _commands = new CommandService();
      _logManager = new LogManager();
      _settings = new Settings(_client);
    }

    private async Task ModifyStatus()
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
      map.AddSingleton(new SaveLoadService(_settings));

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
