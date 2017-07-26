using Discord.Commands;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Roles;
using DragonLore.Managers;
using DragonLore.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DragonLore.Modules
{
  public class CSGOModule : ModuleBase<SocketCommandContext>
  {
    private readonly IBotMessageManager _botMessage;
    private readonly IRoles _roles;

    public CSGOModule(IBotMessageManager botMessage, IRoles roles)
    {
      _botMessage = botMessage;
      _roles = roles;
    }

    [Command("Inventory", RunMode = RunMode.Async)]
    [Summary("Get inventory value of player")]
    public async Task Inventory(string steamID)
    {
      var user = Context.Message.Author as SocketGuildUser;
      string messageContent;

      using (var webClient = new HttpClient())
      {
        var result = await webClient.GetAsync($"http://csgobackpack.net/api/GetInventoryValue/?id={steamID}");
        string jsonContent = await result.Content.ReadAsStringAsync();

        var inventoryData = JsonConvert.DeserializeObject<Inventory>(jsonContent);
        messageContent = inventoryData.Success ?
          $"**Player:** {steamID}{Environment.NewLine}**Inventory value:** {inventoryData.Value} {inventoryData.Currency}{Environment.NewLine}**Items:** {inventoryData.Items}" :
          $"**Error** {Environment.NewLine}Are you sure you entered a correct steamID?{Environment.NewLine}You can get your steamID from your profile url.";
        await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context, user);
      }
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
          messageContent = $"**Error**{Environment.NewLine}Make sure you specify if you want to put the FaceIt role on or off.";
          break;
      }
      await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context, user);
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
          messageContent = $"**Error**{Environment.NewLine}Make sure you specify if you want to put the ESEA role on or off.";
          break;
      }
      await _botMessage.SendAndRemoveEmbedAsync(messageContent, Context, user);
    }
  }
}