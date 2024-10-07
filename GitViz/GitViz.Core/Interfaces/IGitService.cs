﻿namespace GitViz.Core;
public interface IGitService
{
    #region Methods..
    Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetOverallVelocityByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true);

    Dictionary<DateTime, Dictionary<string, (int LinesAdded, int LinesDeleted)>> GetAuthorVelocitiesByMonth(string repositoryPath, DateTimeOffset? startDate, DateTimeOffset? endDate,
        string[]? validExtensions = null, string? branchName = null, bool excludeWhitespace = true);
    #endregion Methods..
}
