using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class GitHubClient
{
    private readonly HttpClient _http;

    public GitHubClient(string userAgent)
    {
        _http = new HttpClient();

        // GitHub API requires a User-Agent header on all requests.
        // It identifies the client making the request.
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

    }

    public async Task<List<string>> GetUserActivity(string username)
    {
        var response = await _http.GetAsync($"https://api.github.com/users/{username}/events");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var eventsJson = JArray.Parse(json);

        var activities = new List<string>();

        foreach (var userEvent in eventsJson)
        {
            string type = (string?)userEvent["type"] ?? "";
            string repo = (string?)userEvent["repo"]?["name"] ?? "unknown-repo";

            string message = type switch
            {
                "PushEvent"   => $"- Pushed {userEvent["payload"]?["commits"]?.Count() ?? 0} commits to {repo}",
                "IssuesEvent" => $"- {userEvent["payload"]?["action"]} an issue in {repo}",
                "WatchEvent"  => $"- Starred {repo}",
                "ForkEvent"   => $"- Forked {repo}",
                _             => $"- {type.Replace("Event", "")} in {repo}"
            };

            activities.Add(message);
        }

        return activities;
    }
}
