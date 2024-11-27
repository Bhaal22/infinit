using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Threading;

public class GitHubClientTests
{
    [Fact]
    public async Task GetRepositoryTree_ReturnsExpectedTreeFromMainBranch()
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var expectedResponse = new GitHubTree
        {
            Sha = "f299b52f39486275a9e6483b60a410e06520c538",
            Url = "https://api.github.com/repos/lodash/lodash/git/trees/f299b52f39486275a9e6483b60a410e06520c538",
            Tree = new List<GitHubTreeItem>
            {
                new GitHubTreeItem
                {
                    Path = ".editorconfig",
                    Mode = "100644",
                    Type = "blob",
                    Sha = "b889a368c15f4eeb444d95fb284cd7c6de5116c2",
                    Size = 243,
                    Url = "https://api.github.com/repos/lodash/lodash/git/blobs/b889a368c15f4eeb444d95fb284cd7c6de5116c2"
                },
                new GitHubTreeItem
                {
                    Path = ".gitattributes", 
                    Mode = "100644",
                    Type = "blob",
                    Sha = "176a458f94e0ea5272ce67c36bf30b6be9caf623",
                    Size = 12,
                    Url = "https://api.github.com/repos/lodash/lodash/git/blobs/176a458f94e0ea5272ce67c36bf30b6be9caf623"
                }
            },
            Truncated = false
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().EndsWith("/main?recursive=1")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var githubClient = new GitHubClient("fake-token")
        {
            _httpClient = httpClient
        };

        var result = await githubClient.GetRepositoryTree("lodash", "lodash");

        Assert.NotNull(result);
        Assert.Equal("f299b52f39486275a9e6483b60a410e06520c538", result.Sha);
        Assert.Equal(2, result.Tree.Count);
        Assert.Equal(".editorconfig", result.Tree[0].Path);
        Assert.Equal(243, result.Tree[0].Size);
        Assert.Equal(".gitattributes", result.Tree[1].Path);
        Assert.Equal(12, result.Tree[1].Size);

        // Verify the request was made to the main branch
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get && 
                req.RequestUri.ToString().EndsWith("/main?recursive=1")),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}


