using Discord;
using Discord.Commands;
using DragonLore.Managers;
using DragonLore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
  public class HelpModule : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly CommandService _commandService;
    private readonly IBotMessageManager _botMessage;

    public HelpModule(IServiceProvider map)
    {
      _settings = map.GetService<Settings>();
      _commandService = map.GetService<CommandService>();
      _botMessage = map.GetService<IBotMessageManager>();
    }

    [Command("Help", RunMode = RunMode.Async)]
    [Summary("List all commands")]
    public async Task Help()
    {
      try
      {
        var user = Context.Message.Author;
        // Check if the user has any rank registered to him from the List<>ranks and remove them.
        var builder = new EmbedBuilder()
        {
          Color = new Color(9912378),
          Description = "These are the commands you can use:"
        };

        foreach (var module in _commandService.Modules)
        {
          string description = null;
          foreach (var cmd in module.Commands)
          {
            var result = await cmd.CheckPreconditionsAsync(Context);
            if (result.IsSuccess)
            {
              if (cmd.Parameters.Count > 0)
              {
                description += $"!{cmd.Aliases.First()} `{string.Join("`, `", cmd.Parameters.Select(p => p.Name))}`\n";
              }
              else
              {
                description += $"!{cmd.Aliases.First()}\n";
              }
            }
          }
          if (!string.IsNullOrWhiteSpace(description))
          {
            builder.AddField(x =>
            {
              x.Name = module.Name;
              x.Value = description;
              x.IsInline = false;
            });
          }
        }
        builder.AddField(x =>
        {
          x.Name = "Where can I use those commands?";
          x.Value = "Please use all commands inside the CSHub discord, since I won't reply or react to DM's";
        });
        await _botMessage.DirectMessageUserAsync("", user, embed: builder.Build());

        var messageContext = "Send you a DM <:csgochicken:306772928626950146> ";
        await _botMessage.SendAndRemoveEmbed(messageContext, Context);
      } catch (Exception e)
      {
        Console.Write(e);
      }
    }

    [Command("Info", RunMode = RunMode.Async)]
    [Summary("Output Info")]
    public async Task Info()
    {
      var Uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
      string messageContent = "";
      messageContent += $"**Name:** Dragon Lore *version 2.0*\n";
      messageContent += $"**Uptime:** {Uptime.Days} Days, {Uptime.Hours} Hours, {Uptime.Minutes} Minutes, {Uptime.Seconds} Seconds.\n";
      messageContent += $"**Discord Servers:** {_settings.Client.Guilds.Count}\n";
      messageContent += $"**Latency:** {_settings.Client.Latency}ms\n";
      messageContent += $"**Memory Usage:** {(Process.GetCurrentProcess().PrivateMemorySize64 / 1024) / 1024}MB\n";
      messageContent += $"**Discord.NET version:** 1.0.1\n";
      messageContent += $"**Developer:** Ryada";
      await _botMessage.SendAndRemoveEmbed(messageContent, Context);
    }
  }
}