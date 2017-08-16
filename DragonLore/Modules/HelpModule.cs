using Discord;
using Discord.Commands;
using DragonLore.Managers;
using DragonLore.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;
        private readonly CommandService _commandService;
        private readonly IBotMessageManager _botMessage;
        private readonly IServiceProvider _map;

        public HelpModule(Settings settings, CommandService commandService, IBotMessageManager botMessage, IServiceProvider map)
        {
            _settings = settings;
            _commandService = commandService;
            _botMessage = botMessage;
            _map = map;
        }

        [Command("Help", RunMode = RunMode.Async)]
        [Summary("List all commands")]
        public async Task Help()
        {
            var user = Context.Message.Author;
            // Check if the user has any rank registered to him from the List<>ranks and remove them.
            var builder = new EmbedBuilder()
              .WithColor(Color.Gold)
              .WithDescription("These are the commands you can use:");

            foreach (var module in _commandService.Modules)
            {
                var description = new StringBuilder();

                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context, _map);
                    if (result.IsSuccess)
                    {
                        description.AppendLine(cmd.Parameters.Count > 0
                            ? $"!{cmd.Aliases.First()} `{string.Join("`, `", cmd.Parameters.Select(p => p.Name))}`"
                            : $"!{cmd.Aliases.First()}");
                    }
                }

                if (description.Length > 0)
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description.ToString();
                    });
                }
            }

            builder.AddField(x =>
            {
                x.Name = "Where can I use those commands?";
                x.Value = "Please use all commands inside the CSHub discord, since I won't reply or react to DM's";
            });

            await _botMessage.DirectMessageUserAsync("", user, builder.Build());

            var messageContext = "Send you a DM <:csgochicken:306772928626950146> ";
            await _botMessage.SendAndRemoveEmbedAsync(messageContext, Context);
        }

        [Command("Info", RunMode = RunMode.Async)]
        [Summary("Output Info")]
        public async Task Info()
        {
            var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

            var messageBuilder = new StringBuilder()
                    .AppendLine("**Name:** Dragon Lore *version 2.0.1*")
                    .AppendLine($"**Uptime:** {uptime.Days} Days, {uptime.Hours} Hours, {uptime.Minutes} Minutes, {uptime.Seconds} Seconds.")
                    .AppendLine($"**Discord Servers:** {_settings.Client.Guilds.Count}")
                    .AppendLine($"**Latency:** {_settings.Client.Latency}ms")
                    .AppendLine($"**Memory Usage:** {(Process.GetCurrentProcess().PrivateMemorySize64 / 1024) / 1024}MB")
                    .AppendLine("**Discord.NET version:** 1.0.1")
                    .AppendLine("**Developer:** Ryada");

            await _botMessage.SendAndRemoveEmbedAsync(messageBuilder.ToString(), Context);
        }
    }
}