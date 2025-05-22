using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Charon.Hosting;

public static class Service
{
    public static void Run<T>(string[] args)
        where T : WorkerBase
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<T>();

        var host = builder.Build();
        host.Run();
    }
}
