namespace GitViz.Api;
public static class RepoEndpoints
{
    #region Fields..
    private static string _tag = "Repo";
    private static string _basePath = "/api/v1/repo";
    #endregion Fields..

    #region Methods..
    public static void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.Ping();
        endpoints.GetLinesChangedByMonth();
    }

    public static void Ping(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{_basePath}/ping", async () =>
        {
            var httpStatusCode = HttpStatusCode.OK;
            object? responseData = null;

            return Results.Json(responseData, statusCode: (int)httpStatusCode);
        })
        .AllowAnonymous()
        .WithName(nameof(Ping))
        .WithTags(_tag);
    }

    public static void GetLinesChangedByMonth(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{_basePath}/getLinesChangedByMonth", async ([FromQuery] string localRepoPath,
                                                                       [FromQuery] string? branchName,
                                                                       [FromQuery] DateTimeOffset? startDate,
                                                                       [FromQuery] DateTimeOffset? endDate,
                                                                       [FromQuery] string[]? fileExtensions,
                                                                       [FromServices] IGitService gitService) =>
        {
            var httpStatusCode = HttpStatusCode.OK;
            object? responseData = null;

            try
            {
                var changesByMonth = gitService.GetRepositoryChangesByMonth(localRepoPath, startDate, endDate, fileExtensions, branchName, true);
                responseData = new GetLinesChangedByMonthResponse() { Json = JsonConvert.SerializeObject(changesByMonth) };
            }
            catch (Exception ex)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return Results.Json(responseData, statusCode: (int)httpStatusCode);
        })
        .AllowAnonymous()
        .Produces<GetLinesChangedByMonthResponse>()
        .WithName(nameof(GetLinesChangedByMonth))
        .WithTags(_tag);
    }
    #endregion Methods..
}
