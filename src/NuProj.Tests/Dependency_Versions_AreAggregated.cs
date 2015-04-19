using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_Versions_AreAggregated : ScenarioBase
    {
        [Fact]
        public async Task Versions_AreAggregated()
        {
            //var result = await this.BuildPackageAsync().AssertSuccessfulBuildAsync();
            var result = await this.BuildPackageAsync("ClassLibrary.NuGet").AssertSuccessfulBuildAsync();
            var expectedVersions = new[]
            {
                    "1.1.20-beta",
                    "1.0.12-alpha",
                    "[0.2, 1.0]",
                    "[0.2, 1.0]",
            };
            var versionSpecs = result.Package.DependencySets
                .SelectMany(x => x.Dependencies)
                .Select(x => x.VersionSpec.ToString());
            Assert.Equal(expectedVersions, versionSpecs);
        }
    }
}
