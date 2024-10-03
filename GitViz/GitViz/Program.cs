namespace GitViz;
public class Program
{
    #region Methods..
    public static void Main(string[] args)
    {
        LogLevel loggingLevel = LogLevel.Error;

        var builder = WebApplication.CreateBuilder(args);

        // Settings
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddUserSecrets<Program>();
        builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
        //builder.Services.Configure<ApplicationConfiguration>(builder.Configuration);

        // Logging
        //builder.Services.AddLogging(loggingBuilder =>
        //{
        //    loggingBuilder.AddProvider(new SqlLoggerProvider((category, level) => level >= loggingLevel, GetMainConnectionString(builder)));
        //});

        // Services
        builder.Services.AddServiceModule<ServicesServiceModule>();
        builder.Services.AddServiceModule<HttpServiceModule>();
        builder.Services.AddServiceModule<SwaggerServiceModule>();

        // Web Host
        builder.WebHost.ConfigureKestrel(ConfigureKestrelHost);

        var app = builder.Build();

        ConfigureApplication(app);
        loggingLevel = LogLevel.Information;

        app.Run();
    }

    private static void ConfigureApplication(WebApplication app)
    {
        app.UseRateLimiter();
        app.UseExceptionHandler("/error");

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }

        app.UseForwardedHeaders();
        //app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors("AllowAll");
        //app.UseAuthentication();
        app.UseAuthorization();

        //app.UseEndpoints(endpoints =>
        //{
        //    MessageEndpoints.Register(endpoints);
        //    UserEndpoints.Register(endpoints);
        //    SystemEndpoints.Register(endpoints);
        //});
    }

    private static void ConfigureKestrelHost(WebHostBuilderContext hostContext, KestrelServerOptions options)
    {
        options.ListenAnyIP(5110);
        options.ListenAnyIP(5111, options => options.UseHttps());
    } 
    #endregion Methods..
}
