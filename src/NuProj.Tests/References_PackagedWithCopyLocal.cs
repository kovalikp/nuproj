using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class References_PackagedWithCopyLocal : ScenarioBase
    {
        [Fact]
        public async Task Package_Exclude()
        {
            var result = await this.BuildPackageAsync("B1.nuget");
            Assert.NotNull(result.Package.GetFile("A2.dll"));
            Assert.Null(result.Package.GetFile("A3.dll")); // CopyLocal=false
            Assert.Null(result.Package.GetFile("A4.dll")); // ExcludeFromNuPkg=true
        }
    }
}
