using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Xunit;

namespace NuProj.Tests.Infrastructure
{
    public static class MSBuild
    {
        public static Task<MSBuildResultAndLogs> RebuildAsync(string projectPath, string projectName = null, IDictionary<string, string> properties = null)
        {

            var target = string.IsNullOrEmpty(projectName) ? "Rebuild" : projectName.Replace('.', '_') + ":Rebuild";
            return MSBuild.ExecuteAsync(projectPath, new[] { target }, properties);
        }

        public static Task<MSBuildResultAndLogs> ExecuteAsync(string projectPath, string targetToBuild, IDictionary<string, string> properties = null)
        {
            return MSBuild.ExecuteAsync(projectPath, new[] { targetToBuild }, properties);
        }

        /// <summary>
        /// Builds a project.
        /// </summary>
        /// <param name="projectPath">The absolute path to the project.</param>
        /// <param name="targetsToBuild">The targets to build. If not specified, the project's default target will be invoked.</param>
        /// <param name="properties">The optional global properties to pass to the project. May come from the <see cref="MSBuild.Properties"/> static class.</param>
        /// <returns>A task whose result is the result of the build.</returns>
        public static async Task<MSBuildResultAndLogs> ExecuteAsync(string projectPath, string[] targetsToBuild = null, IDictionary<string, string> properties = null)
        {
            targetsToBuild = targetsToBuild ?? new string[0];

            var logger = new EventLogger();
            var logLines = new List<string>();
            var parameters = new BuildParameters
            {
                Loggers = new List<ILogger>
                {
                    new ConsoleLogger(LoggerVerbosity.Detailed, logLines.Add, null, null),
                    logger,
                },
            };

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);
                try
                {
                    var hostServices = new HostServices();
                    hostServices.SetNodeAffinity(projectPath, NodeAffinity.OutOfProc);
                    //hostServices = null;
                    var requestData = new BuildRequestData(projectPath, properties ?? Properties.Default, null, targetsToBuild, hostServices);
                    var submission = buildManager.PendBuildRequest(requestData);
                    result = await submission.ExecuteAsync();
                }
                finally
                {
                    buildManager.EndBuild();
                }
            }

            return new MSBuildResultAndLogs(result, logger.LogEvents, logLines);
        }

        /// <summary>
        /// Builds a project.
        /// </summary>
        /// <param name="projectInstance">The project to build.</param>
        /// <param name="targetsToBuild">The targets to build. If not specified, the project's default target will be invoked.</param>
        /// <returns>A task whose result is the result of the build.</returns>
        public static async Task<MSBuildResultAndLogs> ExecuteAsync(ProjectInstance projectInstance, params string[] targetsToBuild)
        {
            targetsToBuild = (targetsToBuild == null || targetsToBuild.Length == 0) ? projectInstance.DefaultTargets.ToArray() : targetsToBuild;

            var logger = new EventLogger();
            var logLines = new List<string>();
            var parameters = new BuildParameters
            {
                //DisableInProcNode = true,
                Loggers = new List<ILogger>
                {
                    new ConsoleLogger(LoggerVerbosity.Detailed, logLines.Add, null, null),
                    logger,
                },
            };

            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                buildManager.BeginBuild(parameters);
                try
                {
                    //var hostServices = new HostServices();
                    //hostServices.SetNodeAffinity(projectInstance.FullPath, NodeAffinity.OutOfProc);
                    //hostServices = null;
                    var requestData = new BuildRequestData(projectInstance, targetsToBuild);
                    var submission = buildManager.PendBuildRequest(requestData);
                    result = await submission.ExecuteAsync();
                }
                finally
                {
                    buildManager.EndBuild();
                }
            }

            return new MSBuildResultAndLogs(result, logger.LogEvents, logLines);
        }

        private static Task<BuildResult> ExecuteAsync(this BuildSubmission submission)
        {
            var tcs = new TaskCompletionSource<BuildResult>();
            submission.ExecuteAsync(s => tcs.SetResult(s.BuildResult), null);
            return tcs.Task;
        }

        /// <summary>
        /// Common properties to pass to a build request.
        /// </summary>
        public static class Properties
        {
            /// <summary>
            /// No properties. The project will be built in its default configuration.
            /// </summary>
            private static readonly ImmutableDictionary<string, string> Empty = ImmutableDictionary.Create<string, string>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// Gets the global properties to pass to indicate where NuProj imports can be found.
            /// </summary>
            public static readonly ImmutableDictionary<string, string> Default = Empty
                .Add("NuProjPath", Assets.NuProjPath)
                .Add("NuProjTasksPath", Assets.NuProjTasksPath)
                .Add("NuGetToolPath", Assets.NuGetToolPath)
                .Add("CustomAfterMicrosoftCommonTargets", Assets.MicrosoftCommonNuProjTargetsPath);

            /// <summary>
            /// The project will build in the same manner as if it were building inside Visual Studio.
            /// </summary>
            public static readonly ImmutableDictionary<string, string> BuildingInsideVisualStudio = Default
                .Add("BuildingInsideVisualStudio", "true");
        }

        private class EventLogger : ILogger
        {
            private IEventSource _eventSource;

            internal EventLogger()
            {
                Verbosity = LoggerVerbosity.Normal;
                LogEvents = new List<BuildEventArgs>();
            }

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }

            public List<BuildEventArgs> LogEvents { get; set; }

            public void Initialize(IEventSource eventSource)
            {               
                _eventSource = eventSource;
                _eventSource.AnyEventRaised += EventSourceAnyEventRaised;
            }

            private void EventSourceAnyEventRaised(object sender, BuildEventArgs e)
            {
                LogEvents.Add(e);
            }

            public void Shutdown()
            {
                _eventSource.AnyEventRaised -= EventSourceAnyEventRaised;
            }
        }
    }
}
