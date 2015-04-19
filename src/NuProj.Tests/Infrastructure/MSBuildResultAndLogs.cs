using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using NuGet;
using Xunit;

namespace NuProj.Tests.Infrastructure
{
    public class MSBuildResultAndLogs
    {
        public MSBuildResultAndLogs(BuildResult result, List<BuildEventArgs> events, IReadOnlyList<string> logLines)
        {
            Result = result;
            LogEvents = events;
            LogLines = logLines;
        }

        public string EntireLog
        {
            get { return string.Join(string.Empty, LogLines); }
        }

        public IEnumerable<BuildErrorEventArgs> ErrorEvents
        {
            get { return LogEvents.OfType<BuildErrorEventArgs>(); }
        }

        public List<BuildEventArgs> LogEvents { get; private set; }

        public IReadOnlyList<string> LogLines { get; private set; }

        public BuildResult Result { get; private set; }

        public IEnumerable<BuildWarningEventArgs> WarningEvents
        {
            get { return LogEvents.OfType<BuildWarningEventArgs>(); }
        }

        public virtual void AssertSuccessfulBuild()
        {
            Assert.False(ErrorEvents.Any(), ErrorEvents.Select(e => e.Message).FirstOrDefault());
            Assert.Equal(BuildResultCode.Success, Result.OverallResult);
        }

        public virtual void AssertUnsuccessfulBuild()
        {
            Assert.Equal(BuildResultCode.Failure, Result.OverallResult);
            Assert.True(ErrorEvents.Any(), ErrorEvents.Select(e => e.Message).FirstOrDefault());
        }

        public class NuPkg : MSBuildResultAndLogs
        {
            private Lazy<IPackage> _package;
            private Lazy<IPackage> _symbolsPackage;

            public NuPkg(BuildResult result, List<BuildEventArgs> events, IReadOnlyList<string> logLines)
                : base(result, events, logLines)
            {
                _package = new Lazy<IPackage>(() => LoadPackage(false), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
                _symbolsPackage = new Lazy<IPackage>(() => LoadPackage(true), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public IPackage Package
            {
                get
                {
                    return _package.Value;
                }
            }

            public IPackage SymbolsPackage
            {
                get
                {
                    return _symbolsPackage.Value;
                }
            }

            public override void AssertSuccessfulBuild()
            {
                base.AssertSuccessfulBuild();
                Assert.NotNull(Package);
            }

            public override void AssertUnsuccessfulBuild()
            {
                base.AssertUnsuccessfulBuild();
                Assert.Null(Package);
            }

            private IPackage LoadPackage(bool symbols)
            {
                if (Result.OverallResult != BuildResultCode.Success)
                    return null;

                if (!Result.ResultsByTarget.ContainsKey("Build"))
                    return null;

                var packagePath = Result["Build"].Items
                    .Select(x => x.ItemSpec)
                    .Where(x => x.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
                    .Where(x => symbols == x.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase))
                    .SingleOrDefault();

                if (packagePath == null)
                    return null;

                return new OptimizedZipPackage(packagePath);
            }
        }

        public class NuSpec : MSBuildResultAndLogs
        {
            private Lazy<Manifest> _manifest;

            public NuSpec(BuildResult result, List<BuildEventArgs> events, IReadOnlyList<string> logLines)
                : base(result, events, logLines)
            {
                _manifest = new Lazy<Manifest>(LoadManifest, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public Manifest Manifest
            {
                get
                {
                    return _manifest.Value;
                }
            }

            public override void AssertSuccessfulBuild()
            {
                base.AssertSuccessfulBuild();
                Assert.NotNull(Manifest);
            }

            public override void AssertUnsuccessfulBuild()
            {
                base.AssertUnsuccessfulBuild();
                Assert.Null(Manifest);
            }

            private Manifest LoadManifest()
            {
                if (Result.OverallResult != BuildResultCode.Success)
                    return null;

                if (!Result.ResultsByTarget.ContainsKey("GenerateNuSpec"))
                    return null;

                var packagePath = Result["GenerateNuSpec"].Items
                    .Select(x => x.ItemSpec)
                    .Where(x => x.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
                    .SingleOrDefault();

                if (packagePath == null)
                    return null;

                using (var file = File.OpenRead(packagePath))
                {
                    return Manifest.ReadFrom(file, false);
                }
            }
        }
        
        public class Target : MSBuildResultAndLogs
        {
            private Lazy<ITargetResult> _targetResult;
            private string _target;

            public Target(BuildResult result, List<BuildEventArgs> events, IReadOnlyList<string> logLines, string target)
                : base(result, events, logLines)
            {
                _target = target;
                _targetResult = new Lazy<ITargetResult>(LoadManifest, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public ITargetResult TargetResult
            {
                get
                {
                    return _targetResult.Value;
                }
            }

            public override void AssertSuccessfulBuild()
            {
                base.AssertSuccessfulBuild();
                Assert.NotNull(TargetResult);
            }

            public override void AssertUnsuccessfulBuild()
            {
                base.AssertUnsuccessfulBuild();
                Assert.Null(TargetResult);
            }

            private ITargetResult LoadManifest()
            {
                if (Result.OverallResult != BuildResultCode.Success)
                    return null;

                if (!Result.ResultsByTarget.ContainsKey(_target))
                    return null;

                return Result[_target];
            }
        }
    }
}