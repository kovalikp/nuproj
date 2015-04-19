using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_Tools_IsNotFiltered : ScenarioBase
    {
        [Fact]
        public async Task Tools_IsNotFiltered()
        {
            var result = await this.BuildPackageAsync("ClassLibrary1.NuGet").AssertSuccessfulBuildAsync();
            //var result = await this.BuildPackageAsync().AssertSuccessfulBuildAsync();
            var expectedFileNames = new[]
            {
                @"lib\net45\ClassLibrary1.dll",
                @"tools\Microsoft.CodeAnalysis.CSharp.Desktop.dll",
                @"tools\Microsoft.CodeAnalysis.CSharp.dll",
                @"tools\Microsoft.CodeAnalysis.Desktop.dll",
                @"tools\Microsoft.CodeAnalysis.dll",
            };
            var files = result.Package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }
    }
}
