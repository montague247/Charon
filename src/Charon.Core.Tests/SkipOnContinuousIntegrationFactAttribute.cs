namespace Charon.Core.Tests
{
    public sealed class SkipOnContinuousIntegrationFactAttribute : FactAttribute
    {
        public SkipOnContinuousIntegrationFactAttribute()
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
                Skip = "No debugger attached";
#else
            Skip = "Skip on CI pipeline";
#endif
        }
    }
}