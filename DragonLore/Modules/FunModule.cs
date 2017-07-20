using Discord.Commands;
using DragonLore.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DragonLore.Modules
{
  public class FunModule : ModuleBase<SocketCommandContext>
  {
    private readonly IBotMessageManager _botMessage;

    public FunModule(IServiceProvider map)
    {
      _botMessage = map.GetService<IBotMessageManager>();
    }

    [Command("Fail", RunMode = RunMode.Async)]
    [Summary("Show a random csgo fail gif")]
    public async Task Fail()
    {
      string filePath = "";
      Random test = new Random();
      switch (test.Next(3))
      {
        case 0:
          filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "FeFailNade.gif");
          break;

        case 1:
          filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "KillTheDefuser.gif");
          break;

        case 2:
          filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "NadeTheAFK.gif");
          break;

        case 3:
          filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Gifs", "Fail", "DuckToDodge.gif");
          break;
      }
      await Context.Channel.SendFileAsync(filePath);
      await _botMessage.RemoveCommandMessageAsync(Context.Message, Context.Channel);
    }
  }
}
