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
    #endregion Methods..
}
