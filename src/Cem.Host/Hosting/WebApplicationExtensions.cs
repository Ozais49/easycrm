namespace Cem.Host.Hosting;

public static class WebApplicationExtensions
{
    public static WebApplication UseCemHost(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseCors();

        app.MapHealthChecks("/health");

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Modules map their endpoint groups here, e.g. app.MapCrmEndpoints();

        return app;
    }
}
