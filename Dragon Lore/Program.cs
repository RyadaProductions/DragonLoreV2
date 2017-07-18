namespace DragonLore
{
  class Program
  {
    static void Main(string[] args)
    {
      new DragonLore().Start().GetAwaiter().GetResult();
    }
  }
}