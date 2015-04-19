using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace NuProj.Tests.Infrastructure
{
    public static class ScenarioExtensions
    {
        public static async Task<MSBuildResultAndLogs.NuSpec> BuildNuspecAsync(this IScenario scenario, string projectName)
        {
            var result = await scenario.BuildProjectAsync(projectName, "GenerateNuSpec");
            return new MSBuildResultAndLogs.NuSpec(result.Result, result.LogEvents, result.LogLines);
        }

        public static async Task<MSBuildResultAndLogs.NuPkg> BuildPackageAsync(this IScenario scenario, string projectName)
        {
            var result = await scenario.BuildProjectAsync(projectName, "Build");
            return new MSBuildResultAndLogs.NuPkg(result.Result, result.LogEvents, result.LogLines);
        }

        [Obsolete]
        public static async Task<MSBuildResultAndLogs.NuPkg> BuildPackageAsync(this IScenario scenario)
        {
            var result = await scenario.BuildSolutionAsync("Build");
            return new MSBuildResultAndLogs.NuPkg(result.Result, result.LogEvents, result.LogLines);
        }

        public static async Task<MSBuildResultAndLogs.Target> BuildTargetAsync(
            this IScenario scenario, 
            string projectName,
            string targetToBuild)
        {
            var result = await scenario.BuildProjectAsync(projectName, targetToBuild);
            return new MSBuildResultAndLogs.Target(result.Result, result.LogEvents, result.LogLines, targetToBuild);
        }

        public static async Task<MSBuildResultAndLogs> BuildProjectAsync(
            this IScenario scenario, string projectName, 
            params string[] targetsToBuild)
        {
            var solutionFullPath = Assets.GetScenarioSolutionPath(scenario.Name);
            var solutionDirectory = Path.GetDirectoryName(solutionFullPath);
            await NuGetHelper.RestorePackagesAsync(solutionDirectory);
            var projectDirectory = Path.Combine(solutionDirectory, projectName, projectName + ".nuproj");

            var properties = scenario.Properties
                .ToImmutableDictionary()
                .SetItem("OutDir", scenario.OutDir)
                .SetItem("IntermediateOutputPath", scenario.IntermediateOutputPath)
                .SetItem("GenerateProjectSpecificOutputFolder", "true");

            var result = await MSBuild.ExecuteAsync(projectDirectory, targetsToBuild, properties);
            return result;
        }

        [Obsolete("This causes build to hang, when building .sln files. Investigate a workaround.")]
        public static async Task<MSBuildResultAndLogs> BuildSolutionAsync(
            this IScenario scenario, 
            params string[] targetsToBuild)
        {
            var solutionFullPath = Assets.GetScenarioSolutionPath(scenario.Name);
            var solutionDirectory = Path.GetDirectoryName(solutionFullPath);
            await NuGetHelper.RestorePackagesAsync(solutionDirectory);

            var properties = scenario.Properties
                .ToImmutableDictionary()
                .SetItem("OutDir", scenario.OutDir)
                .SetItem("IntermediateOutputPath", scenario.IntermediateOutputPath)
                .SetItem("GenerateProjectSpecificOutputFolder", "true");

            var result = await MSBuild.ExecuteAsync(solutionFullPath, targetsToBuild, properties);
            return result;
        }

        public static async Task<T> AssertSuccessfulBuildAsync<T>(this Task<T> msBuildResultAndLogs)
            where T : MSBuildResultAndLogs
        {
            var result = await msBuildResultAndLogs;
            result.AssertSuccessfulBuild();
            return result;
        }
    }
}