using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_DirectDependencies_AreNotPackaged : ScenarioBase
    {
        [Fact]
        public async Task DirectDependencies_AreNotPackaged()
        {
            var result = await this.BuildPackageAsync("A.nuget").AssertSuccessfulBuildAsync();
            var files = result.Package.GetFiles();

            Assert.DoesNotContain(files, x => x.Path.Contains("Newtonsoft.Json.dll"));
            Assert.DoesNotContain(files, x => x.Path.Contains("ServiceModel.Composition.dll"));
            Assert.DoesNotContain(files, x => x.Path.Contains("B3.dll"));
        }
    }
}
