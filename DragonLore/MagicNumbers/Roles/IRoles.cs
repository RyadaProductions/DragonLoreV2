using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DragonLore.MagicNumbers.Roles
{
  public interface IRoles
  {
    SocketRole Admin { get; set; }
    SocketRole Music { get; set; }

    SocketRole Unranked { get; set; }

    SocketRole ESEA { get; set; }
    SocketRole FaceIt { get; set; }
  }
}
