using System.Text.RegularExpressions;

public interface IFileFilter
{
    bool Accept(string path);
}

public interface IGitHubDataFilter
{
    bool Accept(GitHubTreeItem item);
}

public class TreeItemFilter: IGitHubDataFilter
{
    public bool Accept(GitHubTreeItem item)
    {
        return item.Type != "tree" &&
            Regex.Match(item.Path, @"^lib.*\.(js|jst|ts)$").Value != "";
    }
}

public class JsFileFilter : IGitHubDataFilter 
{
    public bool Accept(GitHubTreeItem item)
    {
        return Regex.Match(item.Path, @"^lib.*\.(js|ts)$").Value != "";
    }
}

public class NoFilter : IFileFilter
{
    public bool Accept(string path)
    {
        return true;
    }
}
