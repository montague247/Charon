using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Charon.Hosting;

public static class Service
{
    public static IServiceOptions? Options { get; private set; }

    public static void Run<T>(string name, IServiceOptions options, string[] args)
        where T : WorkerBase
    {
        Options = options;

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            ApplicationName = name ?? typeof(T).Assembly.GetName().Name ?? "Charon.Hosting"
        });
        builder.Services.AddHostedService<T>();

        Log.Information("Start host...");

        var startedUtc = DateTime.UtcNow;

        var host = builder.Build();
        host.Run();

        Log.Information("Host stopped after {Duration}", DateTime.UtcNow - startedUtc);
    }
}
