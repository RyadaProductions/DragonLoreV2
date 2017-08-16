using System.Threading.Tasks;

namespace DragonLore.Rss
{
    internal interface IRssReader
    {
        Task NewsRssAsync(string name, string url);
    }
}