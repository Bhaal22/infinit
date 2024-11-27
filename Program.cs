using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    
    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: program <access_token> <owner> <repo>");
            return;
        }

        string accessToken = args[0];
        string owner = args[1]; 
        string repo = args[2];

        try
        {
            var client = new GitHubClient(accessToken);

            var tree = await client.GetRepositoryTree(owner, repo);
            Console.WriteLine($"\nRepository Tree: {tree}");

            var statistics = new StatisticsCollector();
            foreach (var item in tree.Tree)
            {
                var filter = new JsFileFilter();
                if (!filter.Accept(item.Path)) continue;

                Console.WriteLine($"{item.Type}: {item.Path} ({item.Size} bytes) ({item.Url})");
                var content = await client.GetBlobContent(item.Url);

                statistics.ProcessContent(content);
            }

            statistics.PrintStatistics();
            statistics.PrintSortedStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
