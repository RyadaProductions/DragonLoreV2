using Discord.WebSocket;
using DragonLore.Models;
using System;
using System.Linq;

namespace DragonLore.MagicNumbers.Roles
{
    internal class TestRoles : IRoles
    {
        private ulong AdminId => 280703635342622720;

        private ulong MusicId => 331398290421710849;

        private ulong EseaId => throw new NotImplementedException();

        private ulong FaceItId => throw new NotImplementedException();

        public SocketRole Admin { get; set; }
        public SocketRole Music { get; set; }
        public SocketRole Esea { get; set; }
        public SocketRole FaceIt { get; set; }

        public TestRoles(Settings settings)
        {
            var client = settings.Client;

            if (client.Guilds.Count < 1) return;

            Admin = client.Guilds.First().GetRole(AdminId);
            Music = client.Guilds.First().GetRole(MusicId);
            //Unranked = client.Guilds.First().GetRole(UnrankedId);
            //ESEA = client.Guilds.First().GetRole(ESEAId);
            //FaceIt = client.Guilds.First().GetRole(FaceItId);
        }
    }
}