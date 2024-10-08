namespace GitViz.Core;
public class GitService : IGitService
{
    #region Methods..
    public Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetOverallVelocityByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
    {
        return GitUtils.GetOverallChangesByMonth(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    }

    public Dictionary<DateTime, Dictionary<string, (int LinesAdded, int LinesDeleted)>> GetAuthorVelocityByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
      string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
    {
        return GitUtils.GetAuthorChangesByMonth(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    }

    public Dictionary<string, (int LinesAdded, int LinesDeleted)> GetAuthorVelocityAllTime(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
    {
        return GitUtils.GetAuthorChangesAllTime(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    }
    #endregion Methods..
}
