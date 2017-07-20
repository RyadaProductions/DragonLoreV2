using Discord.Commands;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.MagicNumbers.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DragonLore.PreConditions
{
  internal class RequireAdminPermission : PreconditionAttribute
  {
    private IChannels _channels;
    private IRoles _roles;

    public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
      //@@@ for some reason map does not contain any services so everything i get from it creates a Null exception.
      // This is probably caused by the Discord.NET library, and should need further investigation.
      _channels = services.GetRequiredService<IChannels>();
      _roles = services.GetRequiredService<IRoles>();

      var user = context.Message.Author as SocketGuildUser;
      if (user == null) return Task.FromResult(PreconditionResult.FromError("This command must be used in a guild."));

      if (user.Roles.Contains(_roles.Admin))
        return Task.FromResult(context.Channel.Id == _channels.AdminChannel ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You are not in a admin channel"));
      return Task.FromResult(PreconditionResult.FromError("You must be an admin to run this command."));
    }
  }
}
