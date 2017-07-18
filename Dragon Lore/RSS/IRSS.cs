using System.Threading.Tasks;

namespace DragonLore.RSS
{
  interface IRSS
  {
    Task NewsRSSAsync(string name, string url);
  }
}
