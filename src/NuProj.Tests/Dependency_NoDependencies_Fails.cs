using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_NoDependencies_Fails : ScenarioBase
    {
        [Fact]
        public async Task Solution_Fails()
        {
            var result = await this.BuildPackageAsync("NuGetPackage1");
            //var result = await this.BuildPackageAsync();
            var error = result.ErrorEvents.Single();

            var expectedMessage = "Cannot create a package that has no dependencies nor content.";
            var actualMessage = error.Message;

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
