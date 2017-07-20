using CoreRCON.PacketFormats;
using Discord;
using DragonLore.Managers;
using DragonLore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.Services
{
  public class SaveLoadService
  {
    private readonly Settings _settings;
    private readonly List<string> _roles = new List<string>() { "S1", "S2", "S3", "S4", "SE", "SEM", "GN1", "GN2", "GN3", "GNM", "MG1", "MG2", "MGE", "DMG", "LE", "LEM", "SMFC", "Global" };
    private readonly string _saveFile;

    public SaveLoadService(Settings settings)
    {
      _saveFile = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
      _settings = settings;
    }

    public bool LoadVars()
    {
      try
      {
        if (!File.Exists(_saveFile))
        {
          SaveVars();
        }

        string input = File.ReadAllText(_saveFile);
        var service = JsonConvert.DeserializeObject<Serializing>(input);

        _settings.Ranks = _settings.Client.Guilds.First().Roles.Where(gRole => _roles.Contains(gRole.Name));

        var serversToAdd = new List<string>();

        Parallel.ForEach(service.Servers, async (ip) =>
        {
          ServerQueryInfo info = await new CsgoServerService(_settings).GetServerInfo(ip);
          if (info != null && !_settings.Servers.Contains(ip))
          {
            serversToAdd.Add(ip);
          }
        });
        _settings.Servers = serversToAdd;

        _settings.IsWelcomeMessageOn = service.WelcomeBool;
        _settings.WelcomeMessage = service.WelcomeMessage;

        _settings.LastRSS["gosu"] = service.LastGosuRss;
        _settings.LastRSS["hltv"] = service.LastHltvRss;
        _settings.LastRSS["valve"] = service.LastValveRss;
        return true;
      }
      catch
      {
        return false;
      }
    }

    public bool SaveVars()
    {
      try
      {
        Serializing temp = new Serializing();
        temp.Servers = _settings.Servers;
        temp.WelcomeBool = _settings.IsWelcomeMessageOn;
        temp.WelcomeMessage = _settings.WelcomeMessage;
        temp.LastGosuRss = _settings.LastRSS["gosu"];
        temp.LastHltvRss = _settings.LastRSS["hltv"];
        temp.LastValveRss = _settings.LastRSS["valve"];

        var savableOutput = JsonConvert.SerializeObject(temp);
        File.WriteAllText(_saveFile, savableOutput);
        
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}