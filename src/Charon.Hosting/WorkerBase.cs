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

    protected virtual Task OnStart(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnExecute(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnStop(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var invoker = Environment.GetEnvironmentVariable("INVOCATION_ID");

        if (string.IsNullOrEmpty(invoker))
            Log.Information("Start worker without invocation ID...");
        else
            Log.Information("Start worker with invocation ID '{InvocationId}' ...", invoker);

        await OnStart(stoppingToken);

        Log.Information("Worker started at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            await OnExecute(stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }

        await StopWorkerAsync(stoppingToken);
    }

    private async Task StopWorkerAsync(CancellationToken stoppingToken)
    {
        Log.Information("Stopping worker...");

        await OnStop(stoppingToken);

        Log.Information("Worker stopped at: {Time}", DateTimeOffset.Now);
    }

    private static void OnStarted()
    {
        Log.Information("Service started at: {Time}", DateTimeOffset.Now);
    }

    private  static void OnStopping()
    {
        Log.Information("Service stopping at: {Time}", DateTimeOffset.Now);
    }

    private  static void OnStopped()
    {
        Log.Information("Service stopped at: {Time}", DateTimeOffset.Now);
    }
}
