using Charon.Core.Tests;

namespace Charon.Dojo.Tests
{
    public sealed class DojoRunnerTests
    {
        [SkipOnContinuousIntegrationFact(CheckAttachedDebugger = false)]
        public void Execute()
        {
            DojoRunner.Execute();
        }
    }
}
