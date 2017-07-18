using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dragon_Lore.Handlers;
using Dragon_Lore.MagicNumbers.Roles;
using Dragon_Lore.Preconditions;
using Dragon_Lore.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Dragon_Lore.Models;

namespace Dragon_Lore.Modules
{
  public class RankModule : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly IBotMessageManager _botMessage;
    private readonly IRoles _roles;

    public RankModule(IServiceProvider map)
    {
      _settings = map.GetService<Settings>();
      _botMessage = map.GetService<IBotMessageManager>();

      _roles = map.GetService<IRoles>();
    }

    [Command("Rank", RunMode = RunMode.Async)]
    [Summary("Set your own rank")]
    public async Task ChangeRank([Remainder, Summary("The rank to set")] string newRank = null)
    {
      // Check if parameter has been defined if not stop instantly to avoid doing useless things.
      if (newRank == null)
      {
        await _botMessage.SendAndRemoveEmbed("no rank has been entered. You can set your rank like this: `!rank GN3`", Context);
        return;
      }

      SocketRole oldRole = null;
      var user = Context.Message.Author as IGuildUser;

      // Check if the rank exists
      var newRole = _settings.Ranks.FirstOrDefault(x => x.Name.ToLower() == newRank.ToLower());
      if (newRole == null)
      {
        await _botMessage.SendAndRemoveEmbed($"{newRank} does not exist", Context);
        return;
      }

      // Check if the rank that the user wants to add, is registered and if he does not already have that role
      if (!user.RoleIds.Contains(newRole.Id))
      {
        // check if there is a rank that needs to be removed from the user to make sure they always have 1 rank
        // magicnumber is the Unranked ID
        if (user.RoleIds.Contains(_roles.Unranked))
          oldRole = (Context.Guild.GetRole(_roles.Unranked));
        else
        {
          var oldRank = _settings.Ranks.FirstOrDefault(Rank => user.RoleIds.Contains(Rank.Id));
          if (oldRank != null)
            oldRole = oldRank;
        }
        //change the roles of the user.
        await user.AddRoleAsync(newRole);
        await user.RemoveRoleAsync(oldRole);

        await _botMessage.SendAndRemoveEmbed($"is now {newRole.Name}", Context, user);
      }
      else
        await _botMessage.SendAndRemoveEmbed($"is already {newRole.Name}", Context, user);
    }

    [Command("Unrank", RunMode = RunMode.Async)]
    [Summary("Remove your rank")]
    [Alias("Unranked")]
    public async Task GiveDefaultRank()
    {
      var user = Context.Message.Author as IGuildUser;
      string messageContent;

      var rank = _settings.Ranks.FirstOrDefault(role => user.RoleIds.Contains(role.Id));
      if (rank != null)
      {
        await user.AddRoleAsync(Context.Guild.GetRole(_roles.Unranked));
        await user.RemoveRoleAsync(rank);
        messageContent = "is now unranked";
      }
      else
        messageContent = "is already unranked";

      await _botMessage.SendAndRemoveEmbed(messageContent, Context, user);
    }

    [Command("ListRanks", RunMode = RunMode.Async)]
    [Summary("List all the ranks")]
    [Alias("Ranks", "rankslist")]
    public async Task ShowAllRanks()
    {
      string messageContent;

      if (_settings.Ranks.Count == 0)
        messageContent = "No ranks have been registered";
      else
      {
        var rankList = "";

        foreach (var role in _settings.Ranks)
        {
          rankList += $"- **{role.Name}**: {role.Members.Count()} \n";
        }

        messageContent = $"**Ranks:** \n \n {rankList} \n You can set your rank with **!rank [rankname]**";
      }

      await _botMessage.SendAndRemoveEmbed(messageContent, Context);
    }
  }
}
