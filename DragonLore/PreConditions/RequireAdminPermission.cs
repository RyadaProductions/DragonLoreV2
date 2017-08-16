using Discord.Commands;
using Discord.WebSocket;
using DragonLore.MagicNumbers.Channels;
using DragonLore.MagicNumbers.Roles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.PreConditions
{
    internal class RequireAdminPermission : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var channels = services.GetRequiredService<IChannels>();
            var roles = services.GetRequiredService<IRoles>();

            var user = context.Message.Author as SocketGuildUser;
            if (user == null) return Task.FromResult(PreconditionResult.FromError("This command must be used in a guild."));

            if (user.Roles.Contains(roles.Admin))
                return Task.FromResult(context.Channel.Id == channels.AdminChannel ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("You are not in a admin channel"));
            return Task.FromResult(PreconditionResult.FromError("You must be an admin to run this command."));
        }
    }
}