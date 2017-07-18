using System.Threading.Tasks;

namespace Dragon_Lore.RSS
{
  interface IRSS
  {
    Task NewsRSSAsync(string name, string url);
  }
}
