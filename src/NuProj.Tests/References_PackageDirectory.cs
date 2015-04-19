using System.Linq;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using NuProj.Tests.NuGet;
using Xunit;

namespace NuProj.Tests
{
    public class References_PackageDirectory : ScenarioBase
    {
        [Theory]
        [InlineData("PackageToBuild", new[] { @"build\Tool.dll" }, new string[0])]
        [InlineData("PackageToLib", new[] { @"lib\net45\Tool.dll" }, new string[0])]
        [InlineData("PackageToRoot", new[] { @"Tool.dll", @"Tool.pdb" }, new string[0])]
        [InlineData("PackageToTools", new[] { @"tools\Tool.dll" }, new string[0])]
        [InlineData("PackageDependencyToTools", new[] { @"tools\Tool.dll" }, new[] { "PackageToTools (>= 1.0.0)" })]
        [InlineData("PackageClosureToTools", new[] { @"tools\Tool.dll", @"tools\ToolWithClosure.dll" }, new string[0])]
        [InlineData("PackageToContent", new[] { @"content\Tool.dll", @"content\Tool.pdb" }, new string[0])]
        [InlineData("PackageNuGetDependencyToTools", new[] { @"tools\System.Collections.Immutable.dll", @"tools\ToolWithDependency.dll" }, new string[0])]
        public async Task Package_ToolIsPackaged(
            string packageId,
            string[] expectedFiles,
            string[] expectedDependencies)
        {
            var result = await this.BuildPackageAsync(packageId);
            result.AssertSuccessfulBuild();
            var actualFiles = result.Package.GetFiles().Select(f => f.Path).OrderBy(x => x);
            var actualDependencies = result.Package.DependencySets.NullAsEmpty().Flatten().OrderBy(x => x);
            expectedFiles = expectedFiles.OrderBy(x => x).ToArray();
            expectedDependencies = expectedDependencies.OrderBy(x => x).ToArray();
            Assert.Equal(expectedFiles, actualFiles);
            Assert.Equal(expectedDependencies, actualDependencies);
        }
    }
}