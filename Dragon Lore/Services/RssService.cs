using Discord.WebSocket;
using DragonLore.Managers;
using DragonLore.MagicNumbers.Channels;
using DragonLore.Models;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace DragonLore.Services
{
  public class RssService
  {
    private readonly Settings _settings;
    private readonly SaveLoadService _saveLoadService;
    private readonly IBotMessageManager _botMessage;
    private readonly IChannels _channels;

    public RssService(Settings settings, SaveLoadService saveLoadService, IBotMessageManager botMessage, IChannels channels)
    {
      _settings = settings;
      _saveLoadService = saveLoadService;
      _botMessage = botMessage;
      _channels = channels;
    }

    public async Task CheckNewRss(string rss, string source, SyndicationItem newsItem)
    {
      var channel = _settings.Client.GetChannel(_channels.NewsChannel) as ISocketMessageChannel;
      var newestId = newsItem.Id.ToString();

      if (_settings.LastRSS[rss] != newestId)
      {
        await _botMessage.SendNewsEmbed(source, newsItem, channel);
        _settings.LastRSS[rss] = newestId;
        _saveLoadService.SaveVars();
      }
    }

  }
}
