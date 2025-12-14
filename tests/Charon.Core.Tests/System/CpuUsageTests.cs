using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charon.System;

namespace Charon.Core.Tests.System;

public sealed class CpuUsageTests
{
    [Fact]
    public void FromLinux()
    {
        // Arrange
        var output = "%Cpu(s):  3.6 us, 14.3 sy,  0.0 ni, 82.1 id,  0.0 wa,  0.0 hi,  0.0 si,  0.0 st";

        // Act
        var cpuUsage = CpuUsage.FromLinux(output);

        // Assert
        Assert.NotNull(cpuUsage);
        Assert.Equal(3.6, cpuUsage.User);
        Assert.Equal(14.3, cpuUsage.System);
    }

    [Fact]
    public void FromMac()
    {
        // Arrange
        var output = "CPU usage: 2.88% user, 10.86% sys, 86.25% idle";

        // Act
        var cpuUsage = CpuUsage.FromMac(output);

        // Assert
        Assert.NotNull(cpuUsage);
        Assert.Equal(2.88, cpuUsage.User);
        Assert.Equal(10.86, cpuUsage.System);
    }
}
