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
        endpoints.GetOverallVelocity();
        endpoints.GetAuthorVelocityByMonth();
        endpoints.GetAuthorVelocityAllTime();
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

    public static void GetOverallVelocity(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{_basePath}/getOverallVelocity", async ([FromQuery] string localRepoPath,
                                                                       [FromQuery] string? branchName,
                                                                       [FromQuery] DateTimeOffset? startDate,
                                                                       [FromQuery] DateTimeOffset? endDate,
                                                                       [FromQuery] bool ignoreWhitespace,
                                                                       [FromQuery] string[]? fileExtensions,
                                                                       [FromServices] IGitService gitService) =>
        {
            var httpStatusCode = HttpStatusCode.OK;
            object? responseData = null;

            try
            {
                var changesByMonth = gitService.GetOverallVelocityByMonth(localRepoPath, startDate, endDate, fileExtensions, branchName, ignoreWhitespace);
                responseData = new JsonResponse() { Json = JsonConvert.SerializeObject(changesByMonth) };
            }
            catch (Exception ex)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return Results.Json(responseData, statusCode: (int)httpStatusCode);
        })
        .AllowAnonymous()
        .Produces<JsonResponse>()
        .WithName(nameof(GetOverallVelocity))
        .WithTags(_tag);
    }

    public static void GetAuthorVelocityByMonth(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{_basePath}/getAuthorVelocityByMonth", async ([FromQuery] string localRepoPath,
                                                                       [FromQuery] string? branchName,
                                                                       [FromQuery] DateTimeOffset? startDate,
                                                                       [FromQuery] DateTimeOffset? endDate,
                                                                       [FromQuery] bool ignoreWhitespace,
                                                                       [FromQuery] string[]? fileExtensions,
                                                                       [FromServices] IGitService gitService) =>
        {
            var httpStatusCode = HttpStatusCode.OK;
            object? responseData = null;

            try
            {
                var authorChangesByMonth = gitService.GetAuthorVelocityByMonth(localRepoPath, startDate, endDate, fileExtensions, branchName, ignoreWhitespace);
                responseData = new JsonResponse() { Json = JsonConvert.SerializeObject(authorChangesByMonth) };
            }
            catch (Exception ex)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return Results.Json(responseData, statusCode: (int)httpStatusCode);
        })
        .AllowAnonymous()
        .Produces<JsonResponse>()
        .WithName(nameof(GetAuthorVelocityByMonth))
        .WithTags(_tag);
    }    
    
    public static void GetAuthorVelocityAllTime(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{_basePath}/getAuthorVelocityAllTime", async ([FromQuery] string localRepoPath,
                                                                       [FromQuery] string? branchName,
                                                                       [FromQuery] DateTimeOffset? startDate,
                                                                       [FromQuery] DateTimeOffset? endDate,
                                                                       [FromQuery] bool ignoreWhitespace,
                                                                       [FromQuery] string[]? fileExtensions,
                                                                       [FromServices] IGitService gitService) =>
        {
            var httpStatusCode = HttpStatusCode.OK;
            object? responseData = null;

            try
            {
                var authorChangesAllTime = gitService.GetAuthorVelocityAllTime(localRepoPath, startDate, endDate, fileExtensions, branchName, ignoreWhitespace);
                responseData = new JsonResponse() { Json = JsonConvert.SerializeObject(authorChangesAllTime) };
            }
            catch (Exception ex)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            return Results.Json(responseData, statusCode: (int)httpStatusCode);
        })
        .AllowAnonymous()
        .Produces<JsonResponse>()
        .WithName(nameof(GetAuthorVelocityAllTime))
        .WithTags(_tag);
    }
    #endregion Methods..
}
