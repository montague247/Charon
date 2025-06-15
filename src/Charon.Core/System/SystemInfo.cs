using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Charon.System;

public static class SystemInfo
{
    public static async Task<CpuUsage> GetCpuUsage()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return await GetCpuUsageWindows();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return await GetCpuUsageLinux();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return await GetCpuUsageMac();
        else
            throw new PlatformNotSupportedException("Unsupported operating system");
    }

    [SupportedOSPlatform(nameof(OSPlatform.Windows))]
    private static async Task<CpuUsage> GetCpuUsageWindows()
    {
        var output = await Shell.GetOutput("wmic", ["cpu", "get", "loadpercentage"]);

        return CpuUsage.FromWindows(output);
    }

    [SupportedOSPlatform(nameof(OSPlatform.Linux))]
    private static async Task<CpuUsage> GetCpuUsageLinux()
    {
        var output = await Shell.GetBashOutput(["top", "-bn1", "|", "grep", "'%Cpu(s)'"]);

        return CpuUsage.FromLinux(output);
    }

    [SupportedOSPlatform(nameof(OSPlatform.OSX))]
    private static async Task<CpuUsage> GetCpuUsageMac()
    {
        var output = await Shell.GetBashOutput(["top", "-l", "1", "|", "grep", "'CPU usage'"]);

        return CpuUsage.FromMac(output);
    }
}
