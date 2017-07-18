using System.Threading.Tasks;

namespace DragonLore.RSS
{
  internal interface IRssReader
  {
    Task NewsRSSAsync(string name, string url);
  }
}