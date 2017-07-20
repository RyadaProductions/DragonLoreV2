using Discord.Commands;
using Discord.WebSocket;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.PreConditions;
using DragonLore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
  public class ServerModule : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly IBotMessageManager _botMessage;

    public ServerModule(IServiceProvider map)
    {
      _settings = map.GetService<Settings>();
      _botMessage = map.GetService<IBotMessageManager>();
    }

    [Command("RegisterServer", RunMode = RunMode.Async)]
    [Summary("Register a server")]
    [RequireAdminPermission]
    public async Task AddServerToList([Remainder, Summary("The ip of the server")] string ip)
    {
      string messageContent;

      if (_settings.Servers.Contains(ip))
        messageContent = $"serverlist already contains {ip}";
      else
      {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var serverInfo = await new CsgoServerService(_settings).GetServerInfo(ip);
        stopwatch.Stop();
        var ping = stopwatch.ElapsedMilliseconds;

        if (serverInfo != null)
        {
          _settings.Servers = _settings.Servers.Concat(new[] { ip });

          messageContent = $"**{serverInfo.Name}**\n has successfully been added to the server list. \n `ip: {ip}` || `ping: {ping}ms`";
        }
        else
          messageContent = "Could not add server, no valid ip given";
      }

      await _botMessage.SendAndRemoveEmbed(messageContent, Context);
    }

    [Command("RemoveServer", RunMode = RunMode.Async)]
    [Summary("Remove a server")]
    [RequireAdminPermission]
    public async Task RemoveServerFromList([Remainder, Summary("The server ip to remove")] string ip)
    {
      string messageContent;

      ip = _settings.Servers.FirstOrDefault(y => y == ip);
      if (ip != null)
      {
        _settings.Servers = _settings.Servers.Except(new[] { ip });
        messageContent = $"removed server: {ip}";
      }
      else
        messageContent = $"{ip} is not registered";

      await _botMessage.SendAndRemoveEmbed(messageContent, Context);
    }

    [Command("Servers", RunMode = RunMode.Async)]
    [Summary("List all servers")]
    [Alias("Serverlist")]
    public async Task GenerateServerList()
    {
      var user = Context.Message.Author as SocketGuildUser;

      var messageContent = "";

      if (_settings.Servers.Count() == 0)
        messageContent = "There are no servers registered";
      else
      {
        var onlineServerList = new ConcurrentBag<string>();
        var offlineServerList = new ConcurrentBag<string>();
        Parallel.ForEach(_settings.Servers, async (serverIP) =>
        {
          var stopwatch = new Stopwatch();
          stopwatch.Start();
          var serverInfo = await new CsgoServerService(_settings).GetServerInfo(serverIP);
          stopwatch.Stop();
          var ping = stopwatch.ElapsedMilliseconds;

          if (serverInfo != null)
          {
            var serverString = $"**{serverInfo.Name}**\n `ip: {serverIP}` || `Ping: {ping}ms` || `Players: {serverInfo.Players}/{serverInfo.MaxPlayers}` || `Map: {serverInfo.Map}`\n";
            serverString += $"[Click here to join the server.](steam://connect/{serverIP})\n";
            onlineServerList.Add(serverString);
          }
          else
          {
            var serverString = $"`ip: {serverIP}` || `Status: Offline`\n";
            offlineServerList.Add(serverString);
          }
        });

        var serverList = new List<string>();
        serverList.Add("**Online Servers:** \n \n");
        serverList.AddRange(onlineServerList);
        serverList.Add("\n \n **Offline Servers:**\n \n");
        serverList.AddRange(offlineServerList);

        var count = 0;
        foreach (string server in serverList)
        {
          if (count < 5)
            messageContent += server;
          else
          {
            messageContent += server;
            await _botMessage.DirectMessageUserEmbedAsync(messageContent, user);
            count = 0;
            messageContent = "";
          }
        }
      }
      await _botMessage.DirectMessageUserEmbedAsync(messageContent, user);
    }
  }
}