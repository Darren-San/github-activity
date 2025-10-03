using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        // Determine environment (default = Production)
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        // Load configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Read value from config
        var userAgent = config["GitHub:UserAgent"]
            ?? throw new InvalidOperationException("GitHub:UserAgent is missing in configuration.");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: github-activity <username>");
            return;
        }

        string username = args[0];
        var client = new GitHubClient(userAgent);

        try
        {
            var activities = await client.GetUserActivity(username);

            foreach (var activity in activities)
            {
                Console.WriteLine(activity);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
