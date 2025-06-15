using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Charon.System;

public static class SystemInfo
{
    public static async Task<double?> GetCpuUsage()
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
    private static async Task<double?> GetCpuUsageWindows()
    {
        var output = await Shell.GetOutput("wmic", ["cpu", "get", "loadpercentage"]);

        return output == null ? default : ParseCpuOutput(output);
    }

    [SupportedOSPlatform(nameof(OSPlatform.Linux))]
    private static async Task<double?> GetCpuUsageLinux()
    {
        var output = await Shell.GetBashOutput(["top", "-bn1", "|", "grep", "'%Cpu(s)'"]);

        //%Cpu(s):  3.6 us, 14.3 sy,  0.0 ni, 82.1 id,  0.0 wa,  0.0 hi,  0.0 si,  0.0 st
        return output == null ? default : double.Parse(output.Trim(), CultureInfo.InvariantCulture);
    }

    [SupportedOSPlatform(nameof(OSPlatform.OSX))]
    private static async Task<double?> GetCpuUsageMac()
    {
        var output = await Shell.GetBashOutput(["top", "-l", "1", "|", "grep", "'CPU usage'"]);

        //CPU usage: 2.88% user, 10.86% sys, 86.25% idle
        return output == null ? default : double.Parse(output.Trim(), CultureInfo.InvariantCulture);
    }

    private static double? ParseCpuOutput(string output)
    {
        // Simple parser for Windows output

        foreach (var line in output.Split('\n'))
        {
            if (line.Trim().EndsWith('%') || int.TryParse(line.Trim(), out _))
                return double.Parse(line.Trim(), CultureInfo.InvariantCulture);
        }

        return default; // Indicating failure to parse
    }
}
