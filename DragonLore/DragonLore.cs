using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using DragonLore.Main;

namespace DragonLore
{
  public class DragonLore
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public DragonLore()
    {
      Console.Title = "Dragon Lore Core";

      _client = new DiscordSocketClient(
        new DiscordSocketConfig
        {
          LogLevel = LogSeverity.Info
        });
    }

    public async Task Start()
    {
      var token = Environment.GetEnvironmentVariable("token");

      var installer = new Installer(_client);
      await installer.InstallCommands();
      installer.InstallEvents();

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      await Task.Delay(-1);
    }
  }
}
