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
    public Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetRepositoryChangesByMonth(string localRepositoryPath, string? branchName = null)
        => GitUtils.GetMonthlyChanges(localRepositoryPath, branchName);
    #endregion Methods..
}
