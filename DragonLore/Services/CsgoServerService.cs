using DragonLore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using CoreRCON;
using System.Net;
using System.Threading.Tasks;
using CoreRCON.PacketFormats;

namespace DragonLore.Services
{
  public class CsgoServerService
  {
    private readonly Settings _settings;

    public CsgoServerService(Settings settings)
    {
      _settings = settings;
    }

    public async Task<ServerQueryInfo> GetServerInfo(string ip)
    {
      ushort port = 27015;
      if (ip.Contains(":"))
      {
        port = UInt16.Parse(ip.Substring(ip.IndexOf(':') + 1));
        ip = ip.Substring(0, ip.IndexOf(':'));
      }

      try
      {
        return await ServerQuery.Info(IPAddress.Parse(ip), port);
      }
      catch
      {
        return null;
      }
    }
  }
}
