using Discord;
using Discord.Commands;
using DragonLore.Managers;
using DragonLore.MagicNumbers.Roles;
using DragonLore.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DragonLore.Modules
{
  public class CSGOModule : ModuleBase<SocketCommandContext>
  {
    private readonly IBotMessageManager _botMessage;
    private readonly IRoles _roles;

    public CSGOModule(IServiceProvider map)
    {
      _botMessage = map.GetService<IBotMessageManager>();
      _roles = map.GetService<IRoles>();
    }

    [Command("Inventory", RunMode = RunMode.Async)]
    [Summary("Get inventory value of player")]
    public async Task Inventory(string steamID)
    {
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      System.Net.WebClient wc = new System.Net.WebClient();
      string webData = wc.DownloadString($"http://csgobackpack.net/api/GetInventoryValue/?id={steamID}");
      var inventoryData = JsonConvert.DeserializeObject<Inventory>(webData);
      if (inventoryData.Success)
        messageContent = $"**Player:** {steamID}\n**Inventory value:** {inventoryData.Value} {inventoryData.Currency}\n**Items:** {inventoryData.Items}";
      else
        messageContent = "**Error** \nAre you sure you entered a correct steamID?\nYou can get your steamID from your profile url.";

      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }

    [Command("Faceit", RunMode = RunMode.Async)]
    [Summary("Add or remove the Faceit role")]
    public async Task Faceit(string state = "on")
    {
      // FaceIt role ID      285441809709137921
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      switch (state.ToLower())
      {
        case "on":
          if (!user.Roles.Contains(_roles.FaceIt))
          {
            await user.AddRoleAsync(_roles.FaceIt);
            messageContent = "Now has the FaceIt role.";
          }
          else
            messageContent = "Already has the FaceIt role.";
          break;

        case "off":
          if (user.Roles.Contains(_roles.FaceIt))
          {
            await user.RemoveRoleAsync(_roles.FaceIt);
            messageContent = "Removed the FaceIt role";
          }
          else
            messageContent = "Does not have the FaceIt role.";
          break;

        default:
          messageContent = "**Error** \nMake sure you specify if you want to put the FaceIt role on or off.";
          break;
      }
      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }

    [Command("Esea", RunMode = RunMode.Async)]
    [Summary("Add or remove the ESEA role")]
    public async Task Esea(string state = "on")
    {
      // ESEA role ID      285441890126397450
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      switch (state.ToLower())
      {
        case "on":
          if (!user.Roles.Contains(_roles.ESEA))
          {
            await user.AddRoleAsync(_roles.ESEA);
            messageContent = "Now has the ESEA role.";
          }
          else
            messageContent = "Already has the ESEA role.";
          break;

        case "off":
          if (user.Roles.Contains(_roles.ESEA))
          {
            await user.RemoveRoleAsync(_roles.ESEA);
            messageContent = "Removed the ESEA role";
          }
          else
            messageContent = "Does not have the ESEA role.";
          break;

        default:
          messageContent = "**Error** \nMake sure you specify if you want to put the ESEA role on or off.";
          break;
      }
      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }
  }
}