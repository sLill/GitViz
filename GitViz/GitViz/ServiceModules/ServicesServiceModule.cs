namespace GitViz;
public class ServicesServiceModule : IServiceModule
{
    #region Methods..
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<IGitService, GitService>();
        return services;
    }
    #endregion Methods..
}
