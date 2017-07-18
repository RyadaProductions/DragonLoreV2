using Discord.Commands;
using Dragon_Lore.Handlers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dragon_Lore.Modules
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
      Random test = new Random();
      switch (test.Next(3))
      {
        case 0:
          await Context.Channel.SendFileAsync(Environment.CurrentDirectory + "//Resources//Gifs//Fail//FeFailNade.gif");
          break;
        case 1:
          await Context.Channel.SendFileAsync(Environment.CurrentDirectory + "//Resources//Gifs//Fail//KillTheDefuser.gif");
          break;
        case 2:
          await Context.Channel.SendFileAsync(Environment.CurrentDirectory + "//Resources//Gifs//Fail//NadeTheAFK.gif");
          break;
        case 3:
          await Context.Channel.SendFileAsync(Environment.CurrentDirectory + "//Resources//Gifs//Fail//DuckToDodge.gif");
          break;
      }
      await _botMessage.RemoveCommandMessageAsync(Context.Message, Context.Channel);
    }
  }
}
