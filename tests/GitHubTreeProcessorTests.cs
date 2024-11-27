using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;

public class GitHubTreeProcessorTests
{
    [Fact]
    public async Task ProcessTree_WithFilters_OnlyProcessesMatchingItems()
    {
        var mockClient = new Mock<IGitClient>();
        var mockFilter = new Mock<IGitHubDataFilter>();
        var mockFilter2 = new Mock<IGitHubDataFilter>();
        var statistics = new StatisticsCollector();

        var tree = new GitHubTree
        {
            Tree = new List<GitHubTreeItem>
            {
                new GitHubTreeItem { Path = "test1.js", Url = "url1", Type = "blob" },
                new GitHubTreeItem { Path = "test2.txt", Url = "url2", Type = "blob" },
                new GitHubTreeItem { Path = "test3.js", Url = "url3", Type = "blob" },
                new GitHubTreeItem { Path = ".git", Url = "plop", Type = "tree" }
            }
        };

        mockFilter.Setup(f => f.Accept(It.Is<GitHubTreeItem>(i => i.Path.EndsWith(".js"))))
            .Returns(true);
        mockFilter.Setup(f => f.Accept(It.Is<GitHubTreeItem>(i => !i.Path.EndsWith(".js"))))
            .Returns(false);
        
        mockFilter2.Setup(f => f.Accept(It.Is<GitHubTreeItem>(i => i.Type == "blob")))
            .Returns(true);
        mockFilter2.Setup(f => f.Accept(It.Is<GitHubTreeItem>(i => i.Type == "tree")))
            .Returns(false);

        mockClient.Setup(c => c.GetBlobContent("url1"))
            .ReturnsAsync("content1");
        mockClient.Setup(c => c.GetBlobContent("url3"))
            .ReturnsAsync("content3");

        var processor = new GitHubTreeProcessor(mockClient.Object, new List<IGitHubDataFilter> { mockFilter.Object, mockFilter2.Object });

        await processor.ProcessTree(tree, statistics);

        mockClient.Verify(c => c.GetBlobContent("url1"), Times.Once);
        mockClient.Verify(c => c.GetBlobContent("url2"), Times.Never);
        mockClient.Verify(c => c.GetBlobContent("plop"), Times.Never);
        mockClient.Verify(c => c.GetBlobContent("url3"), Times.Once);
    }


    [Fact]
    public void ShouldProcessItem_WithMultipleFilters_RequiresAllFiltersToAccept()
    {
        var mockClient = new Mock<IGitClient>();
        var mockFilter1 = new Mock<IGitHubDataFilter>();
        var mockFilter2 = new Mock<IGitHubDataFilter>();

        var item = new GitHubTreeItem { Path = "test.js" };

        mockFilter1.Setup(f => f.Accept(item)).Returns(true);
        mockFilter2.Setup(f => f.Accept(item)).Returns(false);

        var processor = new GitHubTreeProcessor(mockClient.Object, 
            new List<IGitHubDataFilter> { mockFilter1.Object, mockFilter2.Object });

        var result = processor.ShouldProcessItem(item);

        Assert.False(result);
        mockFilter1.Verify(f => f.Accept(item), Times.Once);
        mockFilter2.Verify(f => f.Accept(item), Times.Once);
    }
}
