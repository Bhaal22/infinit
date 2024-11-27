using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

public class GitHubContent
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Sha { get; set; }
    public long Size { get; set; }
    public string Type { get; set; }
    public string Download_url { get; set; }
}

public class GitHubTree
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("truncated")]
    public bool Truncated { get; set; }

    [JsonPropertyName("tree")]
    public List<GitHubTreeItem> Tree { get; set; }
}

public class GitHubTreeItem
{
    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("sha")]
    public string Sha { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

class GitHubBlob
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("encoding")]
    public string Encoding { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("sha")]
    public string Sha { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}

public interface IGitClient
{
    public Task<List<GitHubContent>> GetRepositoryContents(string owner, string repo, string path = "");
    public Task<GitHubTree> GetRepositoryTree(string owner, string repo, string sha = "main");
    public Task<string> GetBlobContent(string url);
}

public class GitHubClient: IGitClient
{
    public HttpClient _httpClient {get; set; }
    private readonly string _accessToken;
    private const string BaseUrl = "https://api.github.com";

    public GitHubClient(string accessToken = null)
    {
        _accessToken = accessToken;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        if (!string.IsNullOrEmpty(_accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
        _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubTreeRetriever");
    }

    public async Task<List<GitHubContent>> GetRepositoryContents(string owner, string repo, string path = "")
    {
        var url = $"{BaseUrl}/repos/{owner}/{repo}/contents/{path}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<GitHubContent>>(content);
    }

    public async Task<GitHubTree> GetRepositoryTree(string owner, string repo, string sha = "main")
    {
        var url = $"{BaseUrl}/repos/{owner}/{repo}/git/trees/{sha}?recursive=1";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubTree>(content);
    }

    public async Task<string> GetBlobContent(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var blob = JsonSerializer.Deserialize<GitHubBlob>(content);
        
        byte[] decodedBytes = Convert.FromBase64String(blob.Content);
        return System.Text.Encoding.UTF8.GetString(decodedBytes);
    }
}
