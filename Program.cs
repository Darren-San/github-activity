using GitHubActivityApp.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        //CancellationTokenSource holds unmanaged resources internally (like
        //timers for timeouts). If you don’t dispose it, those resources might
        //linger until garbage collection, which could lead to subtle memory
        //leaks in long-running apps.
        using var cancellationTokenSource = new CancellationTokenSource();

        // Determine environment (default = Production)
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        // Cancel on Ctrl+C
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Cancellation requested...");
            cancellationTokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        // Load configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var gitHubConfig = new GitHubConfiguration
        {
            UserAgent = config.GetSection("GitHub")["UserAgent"]
                        ?? throw new InvalidOperationException("GitHub:UserAgent is missing in configuration.")
        };

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: github-activity <username>");
            return;
        }

        string username = args[0];
        var client = new GitHubClient(gitHubConfig.UserAgent);

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
