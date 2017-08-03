using DragonLore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public bool LoadVarsAsync()
    {
      try
      {
        if (!File.Exists(_saveFile))
        {
          SaveVars();
        }

        var input = File.ReadAllText(_saveFile);
        var service = JsonConvert.DeserializeObject<Serializing>(input);

        _settings.Ranks = _settings.Client.Guilds.First().Roles.Where(gRole => _roles.Contains(gRole.Name));

        _settings.Servers = service.Servers;

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
        var temp = new Serializing()
        {
          Servers = _settings.Servers,
          WelcomeBool = _settings.IsWelcomeMessageOn,
          WelcomeMessage = _settings.WelcomeMessage,
          LastGosuRss = _settings.LastRSS["gosu"],
          LastHltvRss = _settings.LastRSS["hltv"],
          LastValveRss = _settings.LastRSS["valve"]
        };

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