namespace DragonLore
{
    internal class Program
    {
        private static void Main()
        {
            new DragonLore().Start().GetAwaiter().GetResult();
        }
    }
}