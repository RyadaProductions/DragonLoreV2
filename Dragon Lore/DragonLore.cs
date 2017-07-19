using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DragonLore.Main;
using System;
using System.Threading.Tasks;

namespace DragonLore
{
  public class DragonLore
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands = new CommandService();

    public DragonLore()
    {
      Console.Title = "Dragon Lore V2.0";

      _client = new DiscordSocketClient(
        new DiscordSocketConfig
        {
          LogLevel = LogSeverity.Info,
          //WebSocketProvider = WS4NetProvider.Instance
        });
    }

    public async Task Start()
    {
      string token = Environment.GetEnvironmentVariable("token");

      var installer = new Installer(_client);
      await installer.InstallCommands();
      installer.InstallEvents();

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      await Task.Delay(-1);
    }
  }
}