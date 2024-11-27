using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: program [access_token] <owner> <repo>");
            return;
        }

        string accessToken = null;
        string owner;
        string repo;

        if (args.Length >= 3)
        {
            accessToken = args[0];
            owner = args[1];
            repo = args[2];
        }
        else
        {
            owner = args[0];
            repo = args[1];
        }

        try
        {
            var client = new GitHubClient(accessToken);
            var filters = new List<IGitHubDataFilter> { new TreeItemFilter(), new JsFileFilter() };
            var processor = new GitHubTreeProcessor(client, filters);

            var tree = await client.GetRepositoryTree(owner, repo);
            Console.WriteLine($"\nRepository Tree: {tree}");

            var statistics = new StatisticsCollector();
            await processor.ProcessTree(tree, statistics);

            statistics.PrintStatistics();
            statistics.PrintSortedStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
