using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GitHubClient
{
    private readonly HttpClient _http;

    public GitHubClient()
    {
        _http = new HttpClient();

        // required by GitHub API
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp");
    }

    public async Task<List<string>> GetUserActivity(string username)
    {
        var response = await _http.GetAsync($"https://api.github.com/users/{username}/events");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var eventsJson = JsonDocument.Parse(json).RootElement;

        var activities = new List<string>();

        foreach (var userEvent in eventsJson.EnumerateArray())
        {
            string type = userEvent.GetProperty("type").GetString() ?? "";
            string repo = userEvent.GetProperty("repo").GetProperty("name").GetString() ?? "unknown-repo";

            string message = type switch
            {
                "PushEvent"   => $"- Pushed {userEvent.GetProperty("payload").GetProperty("commits").GetArrayLength()} commits to {repo}",
                "IssuesEvent" => $"- {userEvent.GetProperty("payload").GetProperty("action").GetString()} an issue in {repo}",
                "WatchEvent"  => $"- Starred {repo}",
                "ForkEvent"   => $"- Forked {repo}",
                _             => $"- {type.Replace("Event", "")} in {repo}"
            };

            activities.Add(message);
        }

        return activities;
    }
}
