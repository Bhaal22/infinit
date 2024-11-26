using System;
using System.Collections.Generic;
using System.Linq;

public class FileLetterCounter
{
    public static Dictionary<char, int> Parse(string content, Dictionary<char, int> letterCounts)
    {       
        for (int i = 0; i < content.Length; i++)
        {
            if (char.IsLetter(content[i]))
            {
                char letter = char.ToLower(content[i]);
                if (!letterCounts.ContainsKey(letter))
                {
                    letterCounts[letter] = 0;
                }
                letterCounts[letter]++;
            }
        }

        return letterCounts;
    }
}
