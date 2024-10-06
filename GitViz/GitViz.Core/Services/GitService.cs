namespace GitViz.Core;
public class GitService : IGitService
{
    #region Fields..
    #endregion Fields..

    #region Properties..
    #endregion Properties..

    #region Constructors..
    #endregion Constructors..

    #region Methods..
    public Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetRepositoryChangesByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true)
        => GitUtils.GetMonthlyChanges(repositoryPath, startDate, endDate, validExtensions, branchName, excludeWhitespace);
    #endregion Methods..
}
