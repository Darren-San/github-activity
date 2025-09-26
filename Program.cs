using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: github-activity <username>");
            return;
        }

        string username = args[0];
        var client = new GitHubClient();

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
