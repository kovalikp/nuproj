using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using NuProj.Tests.Infrastructure;
using NuProj.Tests.NuGet;
using Xunit;

namespace NuProj.Tests
{
    public class Dependency_MultipleFrameworks_AreResolved : ScenarioBase
    {
        [Fact]
        public async Task MultipleFrameworks_AreResolved()
        {
            var result = await this.BuildPackageAsync("Dependent.nuget").AssertSuccessfulBuildAsync();

            var dependencySet = new[]{
                new PackageDependencySet(VersionUtility.ParseFrameworkName("net40"), new List<PackageDependency>
                    {
                        new PackageDependency("Dependency.nuget", new VersionSpec
                            {
                                IsMinInclusive = true,
                                MinVersion = new SemanticVersion("1.0.0")
                            })
                    }),
                new PackageDependencySet(VersionUtility.ParseFrameworkName("net45"), new List<PackageDependency>
                    {
                        new PackageDependency("Dependency.nuget", new VersionSpec
                            {
                                IsMinInclusive = true,
                                MinVersion = new SemanticVersion("1.0.0")
                            })
                    }),
            };

            Assert.Equal(dependencySet, result.Package.DependencySets, PackageDependencySetComparer.Instance);
        }
    }
}
