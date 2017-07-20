using System.Threading.Tasks;

namespace DragonLore.Rss
{
  internal interface IRssReader
  {
    Task NewsRSSAsync(string name, string url);
  }
}