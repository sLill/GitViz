namespace GitViz.Core;
public interface IGitService
{
    #region Methods..
    Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetRepositoryChangesByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true);
    #endregion Methods..
}
