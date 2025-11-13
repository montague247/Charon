using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Serilog;

namespace Charon.System;

public static partial class Service
{
    [GeneratedRegex(@"systemd (\d+)")]
    private static partial Regex SystemdRegex();

    [GeneratedRegex(@"Version: (\d+)")]
    private static partial Regex VersionRegex();

    public static void Start(string serviceName, IShellOptions shellOptions, bool checkEnabled = true)
    {
        PerformAction(serviceName, shellOptions, ServiceAction.Start, checkEnabled);
    }

    public static void Stop(string serviceName, IShellOptions shellOptions, bool checkEnabled = true)
    {
        PerformAction(serviceName, shellOptions, ServiceAction.Stop, checkEnabled);
    }

    public static void Restart(string serviceName, IShellOptions shellOptions, bool checkEnabled = true)
    {
        PerformAction(serviceName, shellOptions, ServiceAction.Restart, checkEnabled);
    }

    public static void CheckController(IShellOptions shellOptions)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Shell.IsDebianBased())
            CheckControllerDebian(shellOptions);
    }

    private static void PerformAction(string serviceName, IShellOptions shellOptions, ServiceAction action, bool checkEnabled = true)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (Shell.IsDebianBased())
                PerformActionDebian(serviceName, shellOptions, action, checkEnabled);
            else
                throw new NotSupportedException("Service management is only supported on Debian-based Linux distributions at the moment.");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            PerformActionWindows(serviceName, action, checkEnabled);
        else
        {
            throw new PlatformNotSupportedException("Service management is not supported on this platform.");
        }
    }

    [SupportedOSPlatform(nameof(OSPlatform.Linux))]
    private static void PerformActionDebian(string serviceName, IShellOptions shellOptions, ServiceAction action, bool checkEnabled)
    {
        if (checkEnabled)
        {
            var status = Shell.Execute("systemctl", ["is-enabled", serviceName]);

            if (status != 0)
            {
                Shell.SudoExecute("systemctl", ["enable", serviceName], shellOptions);
            }
        }

        Shell.SudoExecute("systemctl", [action.ToString().ToLower(), serviceName], shellOptions);
        Shell.SudoExecute("systemctl", ["status", serviceName], shellOptions);
    }

    [SupportedOSPlatform(nameof(OSPlatform.Windows))]
    private static void PerformActionWindows(string serviceName, ServiceAction action, bool checkEnabled)
    {
        if (checkEnabled)
        {
            var status = Shell.Execute("sc", ["query", serviceName]);

            if (status != 0)
            {
                Shell.Execute("sc", ["config", serviceName, "start=auto"]);
            }
        }

        if (action == ServiceAction.Restart)
        {
            PerformActionWindows(serviceName, ServiceAction.Stop, false);
            PerformActionWindows(serviceName, ServiceAction.Start, false);
        }

        Shell.Execute("sc", [action.ToString().ToLower(), serviceName]);
        Shell.Execute("sc", ["query", serviceName]);
    }

    [SupportedOSPlatform(nameof(OSPlatform.Linux))]
    private static async Task CheckControllerDebian(IShellOptions shellOptions)
    {
        Log.Information("Checking systemd version...");

        var runningVersion = await Shell.GetOutput("systemctl", ["--version"]);
        var installedVersion = await Shell.GetOutput("dpkg", ["-s", "systemd"]);

        if (runningVersion == null || installedVersion == null)
        {
            Log.Error("Could not retrieve systemd version information. Ensure systemd is installed.");
            return;
        }

        var running = ExtractVersion(runningVersion);
        var installed = ExtractInstalledVersion(installedVersion);

        Log.Information("Running systemd version: {Running}, installed: {Installed}", running, installed);

        if (string.Compare(running, installed, StringComparison.Ordinal) == 0)
            return;

        Log.Warning("Running systemd version does not match installed version. Running: {Running}, Installed: {Installed} => reexecute", running, installed);

        Shell.SudoExecute("systemctl", ["daemon-reexec"], shellOptions);
    }

    private static string? ExtractVersion(string input)
    {
        var match = SystemdRegex().Match(input);

        return match.Success ? match.Groups[1].Value : null;
    }

    private static string? ExtractInstalledVersion(string input)
    {
        var match = VersionRegex().Match(input);

        return match.Success ? match.Groups[1].Value : null;
    }
}
