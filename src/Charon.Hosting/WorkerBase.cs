using Microsoft.Extensions.Hosting;
using Serilog;

namespace Charon.Hosting;

public abstract class WorkerBase : BackgroundService
{
    protected WorkerBase(IHostApplicationLifetime hostApplicationLifetime)
    {
        hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
    }

    protected abstract Task OnStart(CancellationToken cancellationToken);

    protected abstract Task OnStop(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Start worker...");

        await OnStart(stoppingToken);

        Log.Information("Worker running at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        Log.Information("Stopping worker...");

        await OnStop(stoppingToken);

        Log.Information("Worker stopped at: {Time}", DateTimeOffset.Now);
    }

    private void OnStarted()
    {
        Log.Information("Service started at: {Time}", DateTimeOffset.Now);
    }

    private void OnStopping()
    {
        Log.Information("Service stopping at: {Time}", DateTimeOffset.Now);
    }

    private void OnStopped()
    {
        Log.Information("Service stopped at: {Time}", DateTimeOffset.Now);
    }
}