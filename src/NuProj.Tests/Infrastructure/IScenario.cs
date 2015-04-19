using System.Collections.Generic;

namespace NuProj.Tests.Infrastructure
{
    public interface IScenario
    {
        string IntermediateOutputPath { get; }

        string Name { get; }

        string OutDir { get; }

        IDictionary<string, string> Properties { get; }
    }
}