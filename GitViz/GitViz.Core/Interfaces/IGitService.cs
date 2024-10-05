namespace GitViz.Core;
public interface IGitService
{
    #region Methods..
    Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetRepositoryChangesByMonth(string localRepositoryPath, string? branchName = null);
    #endregion Methods..
}
