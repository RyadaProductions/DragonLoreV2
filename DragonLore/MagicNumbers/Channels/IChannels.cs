using System;
using System.Collections.Generic;
using System.Text;

namespace DragonLore.MagicNumbers.Channels
{
  public interface IChannels
  {
    ulong NewsChannel { get; }
    ulong AdminChannel { get; }
    ulong MusicChannel { get; }
  }
}
