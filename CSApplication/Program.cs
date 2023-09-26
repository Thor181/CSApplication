namespace CSApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mainModule = new MainModule();
            mainModule.Main().Wait();
        }
    }
}
