using DotNetEnv;
using EdiIngestConsole;
using Microsoft.Extensions.DependencyInjection;


internal class Program
{
    static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        Console.WriteLine(Environment.GetEnvironmentVariable("GREET"));

        // setup dependency injection
        ServiceCollection services = new();
        services.AddTransient<IEdiParser, EdiParser>();

        var provider = services.BuildServiceProvider();


        IEdiParser parser = provider.GetService<IEdiParser>();


        var result = parser.ParseFile("837.txt");

        if (result is null)
            Console.WriteLine("Parse result is null");
        else
        {
            Console.WriteLine($"{result.Count} items parsed.");
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        Console.WriteLine("Press any key to exit.");

        Console.ReadKey();
    }
}
