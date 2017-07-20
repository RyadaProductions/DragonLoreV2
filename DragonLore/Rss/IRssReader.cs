using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DragonLore.Rss
{
  internal interface IRssReader
  {
    Task NewsRSSAsync(string name, string url);
  }
}
