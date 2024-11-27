using System;
using System.Collections.Generic;
using System.Linq;

public class StatisticsCollector
{
    private Dictionary<char, int> _letterCounts;

    public StatisticsCollector()
    {
        _letterCounts = new Dictionary<char, int>();
    }

    public void ProcessContent(string content)
    {
        FileLetterCounter.Parse(content, _letterCounts);
    }

    public Dictionary<char, int> GetStatistics()
    {
        return _letterCounts;
    }

    public IEnumerable<KeyValuePair<char, int>> GetSortedStatistics()
    {
        return _letterCounts.OrderByDescending(x => x.Value);
    }

    public void PrintStatistics()
    {
        foreach (var kvp in _letterCounts)
        {
            Console.WriteLine($"Letter '{kvp.Key}': {kvp.Value} occurrences");
        }
    }

    public void PrintSortedStatistics()
    {
        Console.WriteLine("\nSorted statistics:");
        foreach (var kvp in GetSortedStatistics())
        {
            Console.WriteLine($"Letter '{kvp.Key}': {kvp.Value} occurrences");
        }
    }
}
