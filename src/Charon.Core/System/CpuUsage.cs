using System.Globalization;

namespace Charon.System;

public sealed class CpuUsage
{
    public double? User { get; private set; }

    public double? System { get; private set; }

    public static CpuUsage FromWindows(string? output)
    {
        var cu = new CpuUsage();

        if (output == null)
            return cu;

        foreach (var line in output.Split('\n'))
        {
            if (line.Trim().EndsWith('%') || int.TryParse(line.Trim(), out _))
            {
                cu.System = double.Parse(line.Trim(), CultureInfo.InvariantCulture);

                return cu;
            }
        }

        return cu;
    }

    public static CpuUsage FromLinux(string? output)
    {
        var cu = new CpuUsage();

        if (output == null)
            return cu;

        //%Cpu(s):  3.6 us, 14.3 sy,  0.0 ni, 82.1 id,  0.0 wa,  0.0 hi,  0.0 si,  0.0 st
        var parts = output.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < parts.Length; i++)
        {
            if (string.Compare(parts[i], "us", StringComparison.Ordinal) == 0)
                cu.User = double.Parse(parts[i - 1], CultureInfo.InvariantCulture);
            else if (string.Compare(parts[i], "sy", StringComparison.Ordinal) == 0)
                cu.System = double.Parse(parts[i - 1], CultureInfo.InvariantCulture);
        }

        return cu;
    }

    public static CpuUsage FromMac(string? output)
    {
        var cu = new CpuUsage();

        if (output == null)
            return cu;

        //CPU usage: 2.88% user, 10.86% sys, 86.25% idle
        var parts = output.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].EndsWith('%'))
            {
                if (string.Compare(parts[i + 1], "user", StringComparison.Ordinal) == 0)
                    cu.User = double.Parse(parts[i].TrimEnd('%'), CultureInfo.InvariantCulture);
                else if (string.Compare(parts[i + 1], "sys", StringComparison.Ordinal) == 0)
                    cu.System = double.Parse(parts[i].TrimEnd('%'), CultureInfo.InvariantCulture);
            }
        }

        return cu;
    }

    public override string ToString()
    {
        return $"User: {User?.ToString(CultureInfo.InvariantCulture) ?? "N/A"}%, System: {System?.ToString(CultureInfo.InvariantCulture) ?? "N/A"}%";
    }
}
