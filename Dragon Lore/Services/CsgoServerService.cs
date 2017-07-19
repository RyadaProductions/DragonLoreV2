using DragonLore.Models;
using QueryMaster;
using QueryMaster.GameServer;
using System;

namespace DragonLore.Managers
{
  public class CsgoServerService
  {
    private readonly Settings _settings;

    public CsgoServerService(Settings settings)
    {
      _settings = settings;
    }

    public ServerInfo GetServerInfo(string ip)
    {
      ushort port = 27015;
      if (ip.Contains(":"))
      {
        port = UInt16.Parse(ip.Substring(ip.IndexOf(':') + 1));
        ip = ip.Substring(0, ip.IndexOf(':'));
      }

      try
      {
        using (Server server = ServerQuery.GetServerInstance(EngineType.Source, ip, port, false, 250, 250, 2))
        {
          var info = server.GetInfo();
          return info;
        }
      }
      catch
      {
        return null;
      }
    }
  }
}