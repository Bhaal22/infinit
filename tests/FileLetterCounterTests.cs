using System;
using System.Collections.Generic;
using Xunit;

public class FileLetterCounterTests
{
    [Fact]
    public void Parse_WithMixedContent_CountsLettersCorrectly()
    {
        var content = "Hello, World! 123";
        var letterCounts = new Dictionary<char, int>();

        FileLetterCounter.Parse(content, letterCounts);

        Assert.Equal(3, letterCounts['l']);
        Assert.Equal(2, letterCounts['o']);
        Assert.Equal(1, letterCounts['h']);
        Assert.Equal(1, letterCounts['e']);
        Assert.Equal(1, letterCounts['w']);
        Assert.Equal(1, letterCounts['r']);
        Assert.Equal(1, letterCounts['d']);
        Assert.Equal(7, letterCounts.Count); // Total unique letters
    }

    [Fact]
    public void Parse_WithEmptyString_ReturnsEmptyDictionary()
    {
        var content = "";
        var letterCounts = new Dictionary<char, int>();

        FileLetterCounter.Parse(content, letterCounts);

        Assert.Empty(letterCounts);
    }

    [Fact]
    public void Parse_WithNonLetterCharacters_OnlyCountsLetters()
    {
        var content = "123!@#$%^&*()";
        var letterCounts = new Dictionary<char, int>();

        FileLetterCounter.Parse(content, letterCounts);

        Assert.Empty(letterCounts);
    }
}
