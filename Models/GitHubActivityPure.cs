using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class GitHubActivityPure
{
    public string Type { get; set; } = string.Empty;

    public Repo Repo { get; set; } = new Repo();

    public Payload Payload { get; set; } = new Payload();

    public DateTime CreatedAt { get; set; }

    public string RepoName => Repo?.Name ?? "unknown-repo";

    public int CommitCount => Payload?.Commits?.Count ?? 0;

    public string? Action => Payload?.Action;
}

public class Repo
{
    public string Name { get; set; } = string.Empty;
}

public class Payload
{
    public string? Action { get; set; }

    public List<Commit>? Commits { get; set; }
}

public class Commit
{
    public string? Message { get; set; }
}
