using Discord.Commands;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.PreConditions;
using DragonLore.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
    public class ServerModule : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;
        private readonly IBotMessageManager _botMessage;

        public ServerModule(Settings settings, IBotMessageManager botMessage)
        {
            _settings = settings;
            _botMessage = botMessage;
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
                var serverInfo = await new CsgoServerService().GetServerInfo(ip);
                stopwatch.Stop();
                var ping = stopwatch.ElapsedMilliseconds;

                if (serverInfo != null)
                {
                    _settings.Servers = _settings.Servers.Concat(new[] { ip });

                    messageContent = $"**{serverInfo.Name}**{Environment.NewLine} has successfully been added to the server list.{Environment.NewLine}`ip: {ip}` || `ping: {ping}ms`";
                }
                else
                    messageContent = "Could not add server, no valid ip given";
            }

            await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context);
        }

        [Command("RemoveServer", RunMode = RunMode.Async)]
        [Summary("Remove a server")]
        [RequireAdminPermission]
        public async Task RemoveServerFromList([Remainder, Summary("The server ip to remove")] string ip)
        {
            string messageContent;

            var ipCheck = _settings.Servers.FirstOrDefault(y => y == ip);
            if (ipCheck != null)
            {
                _settings.Servers = _settings.Servers.Except(new[] { ip });
                messageContent = $"removed server: {ip}";
            }
            else
                messageContent = $"{ip} is not registered";

            await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context);
        }

        [Command("Servers", RunMode = RunMode.Async)]
        [Summary("List all servers")]
        [Alias("Serverlist")]
        public async Task GenerateServerList()
        {
            var user = Context.Message.Author;

            var messageContent = new StringBuilder();

            if (_settings.Servers.Any())
                messageContent.Append("There are no servers registered");
            else
            {
                var onlineServerList = new ConcurrentBag<string>();
                var offlineServerList = new ConcurrentBag<string>();

                var tasks = _settings.Servers.Select(async (serverIp) =>
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var serverInfo = await new CsgoServerService().GetServerInfo(serverIp);
                    stopwatch.Stop();
                    var ping = stopwatch.ElapsedMilliseconds;

                    if (serverInfo != null)
                    {
                        var serverString = new StringBuilder($"**{serverInfo.Name}**{Environment.NewLine} `ip: {serverIp}` || `Ping: {ping}ms` || `Players: {serverInfo.Players}/{serverInfo.MaxPlayers}` || `Map: {serverInfo.Map}`{Environment.NewLine}");
                        serverString.Append($"[Click here to join the server.](steam://connect/{serverIp}){Environment.NewLine}");
                        onlineServerList.Add(serverString.ToString());
                    }
                    else
                    {
                        offlineServerList.Add($"`ip: {serverIp}` || `Status: Offline`{Environment.NewLine}");
                    }
                    return true;
                });

                await Task.WhenAll(tasks);

                var serverList = new List<string> {$"**Online Servers:**{Environment.NewLine}{Environment.NewLine}"};
                serverList.AddRange(onlineServerList);
                serverList.Add($"{Environment.NewLine}**Offline Servers:**{Environment.NewLine}{Environment.NewLine}");
                serverList.AddRange(offlineServerList);

                var count = 0;
                foreach (string server in serverList)
                {
                    if (count < 5)
                        messageContent.Append(server);
                    else
                    {
                        messageContent.Append(server);
                        await _botMessage.DirectMessageUserEmbedAsync(messageContent.ToString(), user);
                        count = 0;
                        messageContent.Clear();
                    }
                }
            }
            await _botMessage.DirectMessageUserEmbedAsync(messageContent.ToString(), user);
        }
    }
}