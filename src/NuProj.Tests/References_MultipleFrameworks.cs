using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class References_MultipleFrameworks : ScenarioBase
    {
        [Fact]
        public async Task Package_ReferenceAll()
        {
            var result = await  this.BuildPackageAsync("ReferenceAll");
            var expectedFileNames = new[]
            {
                @"lib\net40\net40.dll",
                @"lib\net45\net40.dll",
                @"lib\net45\net45.dll",
                @"lib\net451\net40.dll",
                @"lib\net451\net45.dll",
                @"lib\net451\net451.dll",
                @"Readme.txt",
            };
            result.AssertSuccessfulBuild();
            var files = result.Package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }

        [Fact]
        public async Task Package_ReferenceNet451()
        {
            var result = await this.BuildPackageAsync("ReferenceNet451");
            var expectedFileNames = new[]
            {
                @"lib\net451\net40.dll",
                @"lib\net451\net45.dll",
                @"lib\net451\net451.dll",
                @"Readme.txt",
            };
            result.AssertSuccessfulBuild();
            var files = result.Package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }
    }
}
