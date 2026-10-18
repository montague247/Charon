using System.Reflection;
using Charon.Console;

namespace Charon.Core.Tests.Console;

public sealed class ArgumentProcessorAttributeTests
{
    [Fact]
    public void Init_Default()
    {
        var actual = new ArgumentProcessorAttribute();
        Assert.Null(actual.Application);
        Assert.Equal("ArgumentProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Init_WithApplication()
    {
        var actual = new ArgumentProcessorAttribute("Custom application");
        Assert.Equal("Custom application", actual.Application);
        Assert.Equal("ArgumentProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Init_WithApplicationAndName()
    {
        var actual = new ArgumentProcessorAttribute("Custom application", "CustomProcessor");
        Assert.Equal("Custom application", actual.Application);
        Assert.Equal("CustomProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var actual = new ArgumentProcessorAttribute("Custom application", "CustomProcessor")
        {
            CaseSensitive = true,
            AllowUnknownArguments = true
        };

        Assert.Equal("Custom application", actual.Application);
        Assert.Equal("CustomProcessor", actual.Name);
        Assert.True(actual.CaseSensitive);
        Assert.True(actual.AllowUnknownArguments);
    }

    [Fact]
    public void AttributeUsage_IsValid()
    {
        var usage = typeof(ArgumentProcessorAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Assembly, usage.ValidOn);
        Assert.False(usage.AllowMultiple);
        Assert.False(usage.Inherited);
    }
}
