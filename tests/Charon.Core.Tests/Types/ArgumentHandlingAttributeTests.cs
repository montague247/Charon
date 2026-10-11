using System.Reflection;
using NetSentinel.Argument;

namespace Charon.Core.Tests.Types;

public sealed class ArgumentHandlingAttributeTests
{
    [Fact]
    public void Init_Default()
    {
        var actual = new ArgumentHandlingAttribute();
        Assert.Null(actual.Description);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Init_WithDescription()
    {
        var actual = new ArgumentHandlingAttribute("Custom argument handling");
        Assert.Equal("Custom argument handling", actual.Description);
        Assert.False(actual.CaseSensitive);
        Assert.False(actual.AllowUnknownArguments);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var actual = new ArgumentHandlingAttribute("Custom argument handling")
        {
            CaseSensitive = true,
            AllowUnknownArguments = true
        };

        Assert.Equal("Custom argument handling", actual.Description);
        Assert.True(actual.CaseSensitive);
        Assert.True(actual.AllowUnknownArguments);
    }

    [Fact]
    public void AttributeUsage_IsValid()
    {
        var usage = typeof(ArgumentHandlingAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Class | AttributeTargets.Assembly, usage.ValidOn);
        Assert.False(usage.AllowMultiple);
        Assert.False(usage.Inherited);
    }
}
