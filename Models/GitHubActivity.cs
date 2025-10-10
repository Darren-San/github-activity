using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubActivityApp.Models
{
    public class GitHubActivity
    {
        public string Type { get; set; } = string.Empty;

        private JObject RepoObj { get; set; } = [];
        //private JObject RepoObj { get; set; } = new JObject();

        public string RepoName => RepoObj["name"]?.ToString() ?? "unknown-repo";

        private JObject PayloadObj { get; set; } = new JObject();

        public string? Action => PayloadObj["action"]?.ToString();

        public int CommitCount => PayloadObj["commits"]?.Count() ?? 0;

        public DateTime CreatedAt { get; set; }
    }
}