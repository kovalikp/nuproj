using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class IncrementalBuild_NuSpecIsNotUpdated : ScenarioBase
    {
        [Fact]
        public async Task IncrementalBuild_NuSpecIsNotUpdated_WhenNothingChanged()
        {
            // Perform first build

            var result1 = await this.BuildProjectAsync("NuGetPackage", "GenerateNuSpec", "Build");
            result1.AssertSuccessfulBuild();

            var nuspecFile = result1.Result["GenerateNuSpec"].Items[0].ItemSpec;

            // Get file stamp of the first nuspec file
            //
            // NOTE: We're asserting that the file exists because otherwise if the file doesn't
            //       exist FileInfo will simply return a placeholder value.

            var fileInfo1 = new FileInfo(nuspecFile);
            Assert.True(fileInfo1.Exists);

            var lastWriteTime1 = fileInfo1.LastWriteTimeUtc;

            // Wait for short period

            await Task.Delay(TimeSpan.FromMilliseconds(300));

            // Perform second build

            var result2 = await this.BuildProjectAsync("NuGetPackage", "Build");
            result2.AssertSuccessfulBuild();

            // Get file stamp of the nuspec file for the second build

            var fileInfo2 = new FileInfo(nuspecFile);
            Assert.True(fileInfo2.Exists);

            var lastWriteTime2 = fileInfo2.LastWriteTimeUtc;

            // The file stamps should match

            Assert.Equal(lastWriteTime1, lastWriteTime2);
        }
    }
}
