﻿using Discord.Audio;
using Discord.WebSocket;
using Dragon_Lore.Handlers;
using Dragon_Lore.Models;
using Newtonsoft.Json;
using QueryMaster.GameServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dragon_Lore.Services
{
  public class SaveLoadService
  {
    private readonly List<string> roles = new List<string>() { "S1", "S2", "S3", "S4", "SE", "SEM", "GN1", "GN2", "GN3", "GNM", "MG1", "MG2", "MGE", "DMG", "LE", "LEM", "SMFC", "Global" };
    private readonly Settings _settings;

    public SaveLoadService(Settings settings)
    {
      _settings = settings;
    }

    public bool LoadVars()
    {
      try
      {
        if (!File.Exists(Environment.CurrentDirectory + "\\settings.json"))
        {
          Console.WriteLine("No save file found, creating a new one.");
          SaveVars();
          return false;
        }

        string input = File.ReadAllText(Environment.CurrentDirectory + "\\settings.json");
        var service = JsonConvert.DeserializeObject<Serializing>(input);

        _settings.Ranks.Clear();
        _settings.Servers.Clear();

        _settings.Ranks.AddRange(_settings.Client.Guilds.First().Roles.Where(gRole => roles.Contains(gRole.Name)));

        Parallel.ForEach(service.Servers, ip =>
        {
          ServerInfo info = new ServerHandler(_settings).GetServerInfo(ip);
          if (info != null && !_settings.Servers.Contains(ip))
          {
            _settings.Servers.Add(ip);
          }
        });

        _settings.IsWelcomeMessageOn = service.WelcomeBool;
        _settings.WelcomeMessage = service.WelcomeMessage;

        _settings.LastRSS["gosu"] = service.LastGosuRss;
        _settings.LastRSS["hltv"] = service.LastHltvRss;
        _settings.LastRSS["valve"] = service.LastValveRss;
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
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
        File.WriteAllText(Environment.CurrentDirectory + "\\settings.json", savableOutput);

        Console.WriteLine("Saved successfully");
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }
  }
}
