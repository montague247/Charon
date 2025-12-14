using System.Diagnostics;
using System.Reflection;

namespace Charon.Core.Tests
{
    public sealed class AssemblyTests
    {
        [Fact]
        public void Test()
        {
            Assert.Equal("xunit.execution.dotnet, Version=2.9.3.0, Culture=neutral, PublicKeyToken=8d05b1bb7a6fdb6c", Assembly.GetCallingAssembly().FullName);
            Assert.Equal("testhost, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", Assembly.GetEntryAssembly()?.FullName);
            Assert.Equal("Charon.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Assembly.GetExecutingAssembly().FullName);
        }
    }
}
