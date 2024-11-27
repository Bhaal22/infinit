using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GitHubTreeProcessor
{
    private readonly List<IGitHubDataFilter> _filters;
    private readonly IGitClient _client;

    public GitHubTreeProcessor(IGitClient client, List<IGitHubDataFilter> filters)
    {
        _client = client;
        _filters = filters;
    }

    public bool ShouldProcessItem(GitHubTreeItem item)
    {
        return _filters.All(filter => filter.Accept(item));
    }

    public async Task<string> GetItemContent(GitHubTreeItem item)
    {
        if (!ShouldProcessItem(item))
        {
            return null;
        }

        return await _client.GetBlobContent(item.Url);
    }

    public async Task ProcessTree(GitHubTree tree, StatisticsCollector statistics)
    {
        foreach (var item in tree.Tree)
        {
            if (!ShouldProcessItem(item))
            {
                continue;
            }

            var content = await GetItemContent(item);
            if (content != null)
            {
                statistics.ProcessContent(content);
            }
        }
    }
}

public interface IStatisticsCollector
{
    void ProcessContent(string content);
}
