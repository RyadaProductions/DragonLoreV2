using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DragonLore.Handlers;
using DragonLore.Preconditions;
using DragonLore.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DragonLore.Models;

namespace DragonLore.Modules
{
  public class AdminModule : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly SaveLoadService _saveLoadService;
    private readonly IBotMessageManager _botMessage;

    public AdminModule(IServiceProvider map)
    {
      _settings = map.GetService<Settings>();
      _saveLoadService = map.GetService<SaveLoadService>();
      _botMessage = map.GetService<IBotMessageManager>();
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
          if (_saveLoadService.LoadVars())
            messageContent = "Settings Loaded";
          else messageContent = "An error occured while loading settings";
          break;
        case "save":
          if (_saveLoadService.SaveVars())
            messageContent = "Settings Saved";
          else messageContent = "An error occured while loading settings";
          break;
        default:
          messageContent = "please specify if i need to load or save";
          break;
      }

      await _botMessage.SendAndRemoveEmbed(messageContent, Context);
    }

    [Command("Kick", RunMode = RunMode.Async)]
    [Summary("kick the user, purge messages, and sends reason")]
    [RequireAdminPermission]
    public async Task KickUser([Summary("@username")] IGuildUser username, [Remainder, Summary("the reason you kick somebody")] string reason)
    {
      var user = Context.Message.Author as IGuildUser;

      var embed = _botMessage.GenerateEmbedAsync($"You have been kicked from: {Context.Guild.Name}\nBy:{user.Username}\nReason:\n{reason}");
      await _botMessage.DirectMessageUserAsync("", username as SocketUser, embed);

      await username.KickAsync(reason);

      await _botMessage.SendEmbedAndRemoveCommand($"has kicked { username.Username}\nReason: { reason}", Context, user);
    }

    [Command("Ban", RunMode = RunMode.Async)]
    [Summary("ban the user for x days, purge messages, and sends reason")]
    [RequireAdminPermission]
    public async Task BanUser([Summary("@username")] IGuildUser username, [Summary("the amount of days to ban the user")] string prune, [Remainder, Summary("the reason you ban somebody")] string reason)
    {
      var user = Context.Message.Author as IGuildUser;
      var pruneInt = Int32.Parse(prune);

      var embed = _botMessage.GenerateEmbedAsync($"You have been banned from: **{Context.Guild.Name}**\n**By:** {user.Username}\n**Time untill unban:** {prune}\n**Reason:**\n{reason}");
      await _botMessage.DirectMessageUserAsync("", username as SocketUser, embed);

      await Context.Guild.AddBanAsync(username, pruneInt, reason);

      await _botMessage.SendEmbedAndRemoveCommand($"has banned: {username.Username}\n**Time untill unban:** {prune}\n**Reason:** {reason}", Context, user);
    }

    [Command("Welcomemessage", RunMode = RunMode.Async)]
    [Summary("set the welcome message")]
    [RequireAdminPermission]
    public async Task ChangeWelcomeMessage([Remainder, Summary("message to set")] string welcomeMessage = "")
    {
      var user = Context.Message.Author as IGuildUser;
      string messageContent;

      if (welcomeMessage != "")
      {
        _settings.WelcomeMessage = welcomeMessage;
        messageContent = $"Welcome message has been set to:\n`{welcomeMessage}`";
      }
      else messageContent = $"Current welcome message:\n`{_settings.WelcomeMessage}`";

      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }

    [Command("Welcome", RunMode = RunMode.Async)]
    [Summary("turn welcome message on or off")]
    [RequireAdminPermission]
    public async Task WelcomeState([Summary("turn it on or off")] string enable = "")
    {
      var user = Context.Message.Author as IGuildUser;
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

      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }
  }
}