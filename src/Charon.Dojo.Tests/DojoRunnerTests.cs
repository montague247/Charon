using Charon.Core.Tests;

namespace Charon.Dojo.Tests
{
    public sealed class DojoRunnerTests
    {
        [SkipOnContinuousIntegrationFact]
        public void Execute()
        {
            DojoRunner.Execute();
        }
    }
}
