using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_Content_IsNotFiltered : ScenarioBase
    {
        [Fact]
        public async Task Content_IsNotFiltered()
        {
            var result = await this.BuildPackageAsync("ClassLibrary1.NuGet").AssertSuccessfulBuildAsync();
            var expectedFileNames = new[]
            {
                @"content\jquery-2.1.1.js",
                @"lib\net45\ClassLibrary1.dll"
            };
            var files = result.Package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }
    }
}
