using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    static bool accept(string path) 
    {
        return Regex.Match(path, @"^lib.*\.(js|ts)$").Value != "";
    }

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

            var letterStatistics = new Dictionary<char, int>();
            foreach (var item in tree.Tree)
            {
                if (!Program.accept(item.Path)) continue;

                Console.WriteLine($"{item.Type}: {item.Path} ({item.Size} bytes) ({item.Url})");
                var content = await client.GetBlobContent(item.Url);

                var dic = FileLetterCounter.Parse(content, letterStatistics);                
            }

            foreach (var kvp in letterStatistics)
            {
                Console.WriteLine($"Letter '{kvp.Key}': {kvp.Value} occurrences");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
