using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class PackageSpecificTests
    {
        [Fact]
        public async Task PackageSpecific_MicrosoftBclBuild()
        {
            await Scenario.RestoreAndBuildAsync(projectName: "TestLibrary");
            var package = await Scenario.RestoreAndBuildSinglePackageAsync(packageId: "Test", 
                properties: MSBuild.Properties.BuildingInsideVisualStudio);
        }
    }
}
