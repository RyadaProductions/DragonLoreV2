using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.PreConditions;
using DragonLore.Services;
using System;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
  public class AdminModule : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly SaveLoadService _saveLoadService;
    private readonly IBotMessageManager _botMessage;

    public AdminModule(Settings settings, SaveLoadService saveLoadService, IBotMessageManager botMessage)
    {
      _settings = settings;
      _saveLoadService = saveLoadService;
      _botMessage = botMessage;
    }

    [Command("Settings", RunMode = RunMode.Async)]
    [Summary("Load/Save settings")]
    [RequireAdminPermission]
    public async Task AdjustSettings([Summary("load/save/reload")] string argument = null)
    {
      string messageContent;

      switch (argument)
      {
        case "load":
          messageContent = await _saveLoadService.LoadVarsAsync() ? "Settings Loaded" : "An error occured while loading settings";
          break;

        case "save":
          messageContent = _saveLoadService.SaveVars() ? "Settings Saved" : "An error occured while loading settings";
          break;

        default:
          messageContent = "please specify if i need to load or save";
          break;
      }

      await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context);
    }

    [Command("Kick", RunMode = RunMode.Async)]
    [Summary("kick the user, purge messages, and sends reason")]
    [RequireAdminPermission]
    public async Task KickUser([Summary("@username")] IGuildUser username, [Remainder, Summary("the reason you kick somebody")] string reason)
    {
      var user = Context.Message.Author as SocketGuildUser;

      var embed = _botMessage.GenerateEmbed($"You have been kicked from: {Context.Guild.Name}{Environment.NewLine}By:{user.Username}\nReason:{Environment.NewLine}{reason}");
      await _botMessage.DirectMessageUserAsync("", username as SocketUser, embed);

      await username.KickAsync(reason);

      await _botMessage.SendEmbedAndRemoveCommandAsync($"has kicked { username.Username}{Environment.NewLine}Reason: { reason}", Context, user);
    }

    [Command("Ban", RunMode = RunMode.Async)]
    [Summary("ban the user for x days, purge messages, and sends reason")]
    [RequireAdminPermission]
    public async Task BanUser([Summary("@username")] IGuildUser username, [Remainder, Summary("the reason you ban somebody")] string reason)
    {
      var user = Context.Message.Author as SocketGuildUser;

      var embed = _botMessage.GenerateEmbed($"You have been banned from: **{Context.Guild.Name}**{Environment.NewLine}**By:** {user.Username}{Environment.NewLine}**Reason:**{Environment.NewLine}{reason}");
      await _botMessage.DirectMessageUserAsync("", username as SocketUser, embed);

      await Context.Guild.AddBanAsync(username, 1, reason);

      await _botMessage.SendEmbedAndRemoveCommandAsync($"has banned: {username.Username}{Environment.NewLine}**Reason:** {reason}", Context, user);
    }

    [Command("Welcomemessage", RunMode = RunMode.Async)]
    [Summary("set the welcome message")]
    [RequireAdminPermission]
    public async Task ChangeWelcomeMessage([Remainder, Summary("message to set")] string welcomeMessage = "")
    {
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      if (welcomeMessage != "")
      {
        _settings.WelcomeMessage = welcomeMessage;
        messageContent = $"Welcome message has been set to:{Environment.NewLine}`{welcomeMessage}`";
      }
      else messageContent = $"Current welcome message:{Environment.NewLine}`{_settings.WelcomeMessage}`";

      await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context, user);
    }

    [Command("Welcome", RunMode = RunMode.Async)]
    [Summary("turn welcome message on or off")]
    [RequireAdminPermission]
    public async Task WelcomeState([Summary("turn it on or off")] string enable = "")
    {
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      switch (enable)
      {
        case "on":
          _settings.IsWelcomeMessageOn = true;
          messageContent = $"welcome message has been turned on";
          break;

        case "off":
          _settings.IsWelcomeMessageOn = false;
          messageContent = $"welcome message has been turned off";
          break;

        default:
          messageContent = $"welcome message is: { (_settings.IsWelcomeMessageOn ? "On" : "Off")}";
          break;
      }

      await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context, user);
    }
  }
}