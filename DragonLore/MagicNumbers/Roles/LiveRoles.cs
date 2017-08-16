using Discord.WebSocket;
using DragonLore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DragonLore.MagicNumbers.Roles
{
    internal class LiveRoles : IRoles
    {
        private readonly Settings _settings;

        private ulong MusicId = 331530125395099651;

        private ulong ESEAId = 285441890126397450;

        private ulong FaceItId = 285441809709137921;

        private ulong AdminId = 278495359406440449;

        public SocketRole Admin { get; set; }

        public SocketRole Music { get; set; }

        public SocketRole Esea { get; set; }

        public SocketRole FaceIt { get; set; }

        public LiveRoles(Settings settings)
        {
            _settings = settings;
        }

        public void GetRoles()
        {
            var client = _settings.Client;
            Admin = client.Guilds.First().GetRole(AdminId);
            Music = client.Guilds.First().GetRole(MusicId);
            Esea = client.Guilds.First().GetRole(ESEAId);
            FaceIt = client.Guilds.First().GetRole(FaceItId);
        }
    }
}