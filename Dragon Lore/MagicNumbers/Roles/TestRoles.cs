using System;

namespace DragonLore.MagicNumbers.Roles
{
  internal class TestRoles : IRoles
  {
    public ulong Admin => 280703635342622720;

    public ulong Music => 331398290421710849;

    public ulong Unranked => throw new NotImplementedException();

    public ulong ESEA => throw new NotImplementedException();

    public ulong FaceIt => throw new NotImplementedException();
  }
}