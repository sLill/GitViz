namespace GitViz.Core;
public class GitService : IGitService
{
    #region Methods..
    public Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetOverallVelocityByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
    {
        return GitUtils.GetOverallMonthlyChanges(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    }

    public Dictionary<DateTime, Dictionary<string, (int LinesAdded, int LinesDeleted)>> GetAuthorVelocitiesByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
      string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
    {
        return GitUtils.GetAuthorMonthlyChanges(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    }

    //private RepositoryCache? GetRepositoryCache(string repositoryPath, string? branchName = null)
    //{
    //    RepositoryCache repositoryCache = null;

    //    string cacheFilename = GetCachedFilename(repositoryPath, branchName);
    //    if (File.Exists(cacheFilename))
    //    {
    //        string serializedData = File.ReadAllText(cacheFilename);
    //        repositoryCache = JsonConvert.DeserializeObject<RepositoryCache>(serializedData);
    //    }

    //    return repositoryCache;
    //}

    //private string GetCachedFilename(string repositoryPath, string? branchName = null)
    //{
    //    string repositoryIdentifier = GitUtils.GetRepositoryIdentifier(repositoryPath);
    //    return string.IsNullOrEmpty(branchName) ? $"{repositoryPath}_{branchName}_{repositoryIdentifier}" : $"{repositoryPath}_{repositoryIdentifier}";
    //}
    #endregion Methods..
}
