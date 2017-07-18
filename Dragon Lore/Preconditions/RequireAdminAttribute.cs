using Discord;
using Discord.Commands;
using DragonLore.MagicNumbers.Channels;
using DragonLore.MagicNumbers.Roles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.Preconditions
{
  internal class RequireAdminPermission : PreconditionAttribute
  {
    private IChannels _channels;
    private IRoles _roles;

    public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
      try
      {
        //@@@ for some reason map does not contain any services so everything i get from it creates a Null exception.
        // This is probably caused by the Discord.NET library, and should need further investigation.
        _channels = services.GetService<IChannels>();
        _roles = services.GetService<IRoles>();

        var user = context.Message.Author as IGuildUser;

        if (user.RoleIds.Contains(_roles.Admin))
        {
          if (context.Channel.Id == _channels.AdminChannel)
            return await Task.FromResult(PreconditionResult.FromSuccess());
          return await Task.FromResult(PreconditionResult.FromError("You are not in a admin channel"));
        }
        return await Task.FromResult(PreconditionResult.FromError("You must be an admin to run this command."));
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return await Task.FromResult(PreconditionResult.FromError("You must be an admin to run this command."));
      }
    }
  }
}