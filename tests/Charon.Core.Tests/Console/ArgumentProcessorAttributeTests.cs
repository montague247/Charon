using System.Reflection;
using Charon.Console;

namespace Charon.Core.Tests.Console;

public sealed class ArgumentProcessorAttributeTests
{
    [Fact]
    public void Init_Default()
    {
        var actual = new ArgumentProcessorAttribute();
        Assert.Null(actual.Description);
        Assert.Equal("ArgumentProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Init_WithDescription()
    {
        var actual = new ArgumentProcessorAttribute("Custom argument handling");
        Assert.Equal("Custom argument handling", actual.Description);
        Assert.Equal("ArgumentProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Init_WithDescriptionAndName()
    {
        var actual = new ArgumentProcessorAttribute("Custom argument handling", "CustomProcessor");
        Assert.Equal("Custom argument handling", actual.Description);
        Assert.Equal("CustomProcessor", actual.Name);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var actual = new ArgumentProcessorAttribute("Custom argument handling", "CustomProcessor")
        {
            CaseSensitive = true,
            AllowUnknownArguments = true
        };

        Assert.Equal("Custom argument handling", actual.Description);
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
