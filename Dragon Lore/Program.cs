namespace Dragon_Lore
{
  class Program
  {
    static void Main(string[] args)
    {
      new DragonLore().Start().GetAwaiter().GetResult();
    }
  }
}