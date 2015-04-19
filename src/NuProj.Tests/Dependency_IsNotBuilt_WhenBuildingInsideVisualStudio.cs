using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_IsNotBuilt_WhenBuildingInsideVisualStudio : ScenarioBase
    {
        [Theory]
        [InlineData("Build")]
        [InlineData("Clean")]
        [InlineData("Rebuild")]
        public async Task IsNotBuilt_WhenBuildingInsideVisualStudio(string target)
        {
            Properties = Properties.AddRange(MSBuild.Properties.BuildingInsideVisualStudio);
            var result = await this.BuildPackageAsync("NuGetPackage").AssertSuccessfulBuildAsync();
        }
    }
}
