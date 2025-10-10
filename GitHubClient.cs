using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GitHubActivityApp.Models;

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
        var events = JsonConvert.DeserializeObject<List<GitHubActivity>>(json) ?? [];

        var activities = new List<string>();

        foreach (var userEvent in events)
        {
            string message = userEvent.Type switch
            {
                "PushEvent"   => $"- Pushed {userEvent.CommitCount} commits to {userEvent.RepoName}",
                "IssuesEvent" => $"- {userEvent.Action} an issue in {userEvent.RepoName}",
                "WatchEvent"  => $"- Starred {userEvent.RepoName}",
                "ForkEvent"   => $"- Forked {userEvent.RepoName}",
                _             => $"- {userEvent.Type.Replace("Event", "")} in {userEvent.RepoName}"
            };

            activities.Add(message);
        }

        return activities;
    }
}
