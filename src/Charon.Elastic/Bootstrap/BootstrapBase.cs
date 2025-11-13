using System.Diagnostics;
using Serilog;

namespace Charon.Elastic.Bootstrap;

public abstract class BootstrapBase(string basePath, string name)
{
    private readonly string _name = name;
    private CancellationToken _cancellationToken;

    protected string BasePath { get; } = basePath;

    public bool LogOutput { get; set; } = true;

    protected abstract string Command { get; }

    public virtual async Task Execute(CancellationToken cancellationToken, params string[] args)
    {
        _cancellationToken = cancellationToken;

        var psi = new ProcessStartInfo(Path.Combine(BasePath, "bin", Command))
        {
            WorkingDirectory = BasePath,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (var arg in args)
        {
            psi.ArgumentList.Add(arg);
        }

        using var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += ProcessOutput;
        process.ErrorDataReceived += ProcessError;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            try { if (!process.HasExited) process.Kill(true); } catch { }
            throw;
        }
    }

    protected virtual void ProcessOutput(string output, CancellationToken cancellationToken)
    {
    }

    protected virtual void ProcessError(string error, CancellationToken cancellationToken)
    {
    }

    private void ProcessOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        if (LogOutput)
            Log.Information("[{Name}] {Data}", _name, e.Data);

        ProcessOutput(e.Data, _cancellationToken);
    }

    private void ProcessError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        Log.Error("[{Name}] {Data}", _name, e.Data);

        ProcessError(e.Data, _cancellationToken);
    }
}
