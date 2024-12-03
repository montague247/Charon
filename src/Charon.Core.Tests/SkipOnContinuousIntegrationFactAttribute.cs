namespace Charon.Core.Tests
{
    public sealed class SkipOnContinuousIntegrationFactAttribute : FactAttribute
    {
        public SkipOnContinuousIntegrationFactAttribute()
        {
#if DEBUG
            if (CheckAttachedDebugger &&
                !System.Diagnostics.Debugger.IsAttached)
                Skip = "No debugger attached";
#else
            Skip = "Skip on CI pipeline";
#endif
        }

        public bool CheckAttachedDebugger { get; set; } = true;
    }
}
