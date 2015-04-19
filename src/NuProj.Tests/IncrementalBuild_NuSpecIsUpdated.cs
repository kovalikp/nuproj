using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class IncrementalBuild_NuSpecIsUpdated : ScenarioBase
    {
        [Fact]
        public async Task IncrementalBuild_NuSpecIsUpdated_WhenGlobalPropertiesChange()
        {
            const string expectedDescription1 = "First";
            const string expectedDescription2 = "Second";

            // Perform first build

            Properties = Properties.SetItem("Description", expectedDescription1);
            var result1 = await this.BuildPackageAsync("NuGetPackage").AssertSuccessfulBuildAsync();

            var actualDescription1 = result1.Package.Description;

            // Perform second build

            Properties = Properties.SetItem("Description", expectedDescription2);
            var result2 = await this.BuildPackageAsync("NuGetPackage").AssertSuccessfulBuildAsync();

            var actualDescription2 = result2.Package.Description;

            Assert.Equal(expectedDescription1, actualDescription1);
            Assert.Equal(expectedDescription2, actualDescription2);
        }
    }
}
