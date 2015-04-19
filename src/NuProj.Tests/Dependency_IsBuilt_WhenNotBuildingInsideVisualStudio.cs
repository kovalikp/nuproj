using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_IsBuilt_WhenNotBuildingInsideVisualStudio : ScenarioBase
    {
        [Theory]
        [InlineData("Build")]
        [InlineData("Clean")]
        [InlineData("Rebuild")]
        public async Task IsBuilt_WhenNotBuildingInsideVisualStudio(string target)
        {
            var result = await this.BuildProjectAsync("NuGetPackage", target).AssertSuccessfulBuildAsync();

            var warnings = result.WarningEvents.ToArray();

            if (target == "Rebuild")
            {
                Assert.Equal(4, warnings.Length);
                Assert.Equal("CsProj dependency Target Called: Clean", warnings[0].Message);
                Assert.Equal("NuProj dependency Target Called: Clean", warnings[1].Message);
                Assert.Equal("CsProj dependency Target Called: Build", warnings[2].Message);
                Assert.Equal("NuProj dependency Target Called: Build", warnings[3].Message);
            }
            else
            {
                Assert.Equal(2, warnings.Length);
                Assert.Equal("CsProj dependency Target Called: " + target, warnings[0].Message);
                Assert.Equal("NuProj dependency Target Called: " + target, warnings[1].Message);
            }
        }
    }
}
