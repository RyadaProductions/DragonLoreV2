using Dragon_Lore.Models;
using Dragon_Lore.Services;
using QueryMaster;
using QueryMaster.GameServer;
using System;

namespace Dragon_Lore.Handlers
{
  public class ServerHandler
  {
    private readonly Settings _settings;

    public ServerHandler(Settings settings)
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
