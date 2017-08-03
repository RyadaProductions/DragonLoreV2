using CoreRCON;
using CoreRCON.PacketFormats;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DragonLore.Services
{
  public class CsgoServerService
  {
    public CsgoServerService()
    {
    }

    public async Task<SourceQueryInfo> GetServerInfo(string ip)
    {
      ushort port = 27015;
      if (ip.Contains(":"))
      {
        port = UInt16.Parse(ip.Substring(ip.IndexOf(':') + 1));
        ip = ip.Substring(0, ip.IndexOf(':'));
      }

      try
      {
        return await ServerQuery.Info(IPAddress.Parse(ip), port, ServerQuery.ServerType.Source) as SourceQueryInfo;
      }
      catch
      {
        return null;
      }
    }
  }
}