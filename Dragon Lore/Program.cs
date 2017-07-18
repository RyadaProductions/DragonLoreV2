namespace DragonLore
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      new DragonLore().Start().GetAwaiter().GetResult();
    }
  }
}