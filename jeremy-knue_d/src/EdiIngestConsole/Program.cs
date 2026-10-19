using DotNetEnv;
using EdiIngestConsole;

 
internal class Program
{
    static void Main(string[] args)
    { 
        DotNetEnv.Env.Load();

       Console.WriteLine(Environment.GetEnvironmentVariable("GREET"));

       

        EdiParser parser = new();
 

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

       Console.WriteLine("Press enter to exit.");

       Console.ReadLine();
    }
}
