using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_IndirectDependencies_AreNotPackaged : ScenarioBase
    {
        [Theory]
        [InlineData(@"A.nuget", "A.nuget")]
        public async Task GetPackageIdentity_ResturnsCorrectValue(string projectToBuild, string identity)
        {
            // Arrange
            const string target = "GetPackageIdentity";

            // Act
            var result = await this.BuildTargetAsync(projectToBuild, target).AssertSuccessfulBuildAsync();

            // Assert
            Assert.Single(result.TargetResult.Items);
            Assert.Equal(result.TargetResult.Items[0].ItemSpec, identity);
        }

        [Fact]
        public async Task IndirectDependencies_AreNotPackaged()
        {
            var result = await this.BuildPackageAsync("A.nuget");
            var files = result.Package.GetFiles();

            Assert.DoesNotContain(files, x => x.Path.Contains("Newtonsoft.Json.dll"));
            Assert.DoesNotContain(files, x => x.Path.Contains("ServiceModel.Composition.dll"));
            Assert.DoesNotContain(files, x => x.Path.Contains("B3.dll"));
        }

    }
}
