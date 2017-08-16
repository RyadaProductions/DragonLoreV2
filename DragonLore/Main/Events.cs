using Discord;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.MagicNumbers.Roles;
using DragonLore.Managers;
using DragonLore.Models;
using DragonLore.Rss;
using DragonLore.Services;
using DragonLore.UpcomingMatches;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DragonLore.Main
{
    internal class Events
    {
        private readonly SaveLoadService _saveLoadService;
        private readonly Settings _settings;
        private readonly IServiceProvider _map;

        private readonly IBotMessageManager _botMessage;
        private readonly IRoles _roles;
        private readonly IChannels _channels;
        private readonly CommandChecker _commandChecker;
        private readonly LogManager _logManager;

        public Events(IServiceProvider map)
        {
            _map = map;

            _commandChecker = new CommandChecker(_map);

            _settings = map.GetService<Settings>();
            _saveLoadService = map.GetService<SaveLoadService>();
            _botMessage = map.GetService<IBotMessageManager>();
            _roles = map.GetService<IRoles>();
            _channels = map.GetService<IChannels>();
            _logManager = map.GetService<LogManager>();
        }

        public async Task ConnectedToGuild(SocketGuild arg)
        {
            if (_saveLoadService.LoadVarsAsync())
                await _logManager.Logger(new LogMessage(LogSeverity.Info, "Settings", "Settings loaded succesfully."));
            else
                await _logManager.Logger(new LogMessage(LogSeverity.Warning, "Settings", "Error loading Settings, this is normal if it is a fresh installation."));

            IRssReader mainRss = new RssReader(_map.GetService<RssService>(), _logManager);
            var mainParser = new HltvParser(_settings, _channels, _botMessage);

            _settings.Client.GuildAvailable -= ConnectedToGuild;
        }

        public async Task WelcomeHandler(SocketGuildUser arg)
        {
            var user = arg;
            if (user == null) return;

            if (!_settings.IsWelcomeMessageOn || _settings.WelcomeMessage == "") return;
            await _botMessage.DirectMessageUserEmbedAsync(_settings.WelcomeMessage, user);
        }

        public async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState voiceBefore, SocketVoiceState voiceAfter)
        {
            var guildUser = user as SocketGuildUser;
            if (voiceAfter.VoiceChannel.Id == _channels.MusicChannel) await guildUser.AddRoleAsync(_roles.Music);
            else await guildUser.RemoveRoleAsync(_roles.Music);
        }

        public async Task CmdHandler(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;

            if (await _commandChecker.Spam(msg)) return;
            await _commandChecker.Command(msg);
        }
    }
}