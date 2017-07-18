using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Dragon_Lore.Main;
using System;
using System.Threading.Tasks;

namespace Dragon_Lore
{
  public class DragonLore
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands = new CommandService();

    public DragonLore()
    {
      Console.Title = "Dragon Lore V2.0";

      _client = new DiscordSocketClient(new DiscordSocketConfig {
        LogLevel = LogSeverity.Info,
        WebSocketProvider = WS4NetProvider.Instance
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

    private Task Logger(LogMessage message)
    {
      var cc = Console.ForegroundColor;
      switch (message.Severity)
      {
        case LogSeverity.Critical:
        case LogSeverity.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case LogSeverity.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
        case LogSeverity.Info:
          Console.ForegroundColor = ConsoleColor.White;
          break;
        case LogSeverity.Verbose:
        case LogSeverity.Debug:
          Console.ForegroundColor = ConsoleColor.DarkGray;
          break;
      }
      Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
      Console.ForegroundColor = cc;
      return Task.CompletedTask;
    }
  }
}
