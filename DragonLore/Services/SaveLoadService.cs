using DragonLore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DragonLore.MagicNumbers.Roles;

namespace DragonLore.Services
{
    public class SaveLoadService
    {
        private readonly Settings _settings;
        private readonly List<string> _ranks = new List<string>() { "S1", "S2", "S3", "S4", "SE", "SEM", "GN1", "GN2", "GN3", "GNM", "MG1", "MG2", "MGE", "DMG", "LE", "LEM", "SMFC", "Global" };
        private readonly string _saveFile;
        private readonly IRoles _roles;

        public SaveLoadService(Settings settings, IRoles roles)
        {
            _saveFile = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            _settings = settings;
            _roles = roles;
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

                _settings.Ranks = _settings.Client.Guilds.First().Roles.Where(gRole => _ranks.Contains(gRole.Name)).ToList();

                _settings.Servers = service.Servers;

                _settings.IsWelcomeMessageOn = service.WelcomeBool;
                _settings.WelcomeMessage = service.WelcomeMessage;

                _settings.LastRss["gosu"] = service.LastGosuRss;
                _settings.LastRss["hltv"] = service.LastHltvRss;
                _settings.LastRss["valve"] = service.LastValveRss;
                
                ((LiveRoles)_roles).GetRoles();
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
                    LastGosuRss = _settings.LastRss["gosu"],
                    LastHltvRss = _settings.LastRss["hltv"],
                    LastValveRss = _settings.LastRss["valve"]
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