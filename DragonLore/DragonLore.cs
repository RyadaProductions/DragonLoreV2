using Discord;
using Discord.WebSocket;
using DragonLore.Main;
using System;
using System.Threading.Tasks;

namespace DragonLore
{
  public class DragonLore
  {
    private readonly DiscordSocketClient _client;

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