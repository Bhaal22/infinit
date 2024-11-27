using System.Text.RegularExpressions;

public interface IFileFilter
{
    bool Accept(string path);
}

public class JsFileFilter : IFileFilter 
{
    public bool Accept(string path)
    {
        return Regex.Match(path, @"^lib.*\.(js|ts)$").Value != "";
    }
}
