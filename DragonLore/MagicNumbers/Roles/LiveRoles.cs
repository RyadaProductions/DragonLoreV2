using Discord.WebSocket;
using DragonLore.Models;
using System.Linq;

namespace DragonLore.MagicNumbers.Roles
{
  internal class LiveRoles : IRoles
  {
    private ulong MusicId = 331530125395099651;

    private ulong UnrankedId = 282823369663971328;

    private ulong ESEAId = 285441890126397450;

    private ulong FaceItId = 285441809709137921;

    private ulong AdminId = 278495359406440449;

    public SocketRole Admin { get; set; }

    public SocketRole Music { get; set; }

    public SocketRole Unranked { get; set; }

    public SocketRole ESEA { get; set; }

    public SocketRole FaceIt { get; set; }

    public LiveRoles(Settings settings)
    {
      var client = settings.Client;

      Admin = client.Guilds.First().GetRole(AdminId);
      Music = client.Guilds.First().GetRole(MusicId);
      Unranked = client.Guilds.First().GetRole(UnrankedId);
      ESEA = client.Guilds.First().GetRole(ESEAId);
      FaceIt = client.Guilds.First().GetRole(FaceItId);
    }
  }
}