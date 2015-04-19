using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

namespace NuProj.Tests.Infrastructure
{
    public abstract class ScenarioBase : IScenario, IDisposable
    {
        private bool _disposed;
        private string _projectDirectory;

        protected ScenarioBase()
        {
            Name = GetType().Name;
            var tempPath = Path.GetTempPath();
            var randomFileName = Path.GetRandomFileName();
            _projectDirectory = Path.Combine(tempPath, "NuProj.Tests", randomFileName);
            Properties = MSBuild.Properties.Default;
            Directory.CreateDirectory(_projectDirectory);
        }

        protected ScenarioBase(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
        }

        public string IntermediateOutputPath
        {
            get
            {
                return Path.Combine(_projectDirectory, "obj\\");
            }
        }

        IDictionary<string, string> IScenario.Properties
        {
            get { return Properties; }
        }

        public string Name { get; private set; }

        public string OutDir
        {
            get
            {
                return Path.Combine(_projectDirectory, "bin\\");
            }
        }

        protected ImmutableDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">If <code>false</code>, cleans up native resources.
        /// If <code>true</code> cleans up both managed and native resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                try
                {
                    //DeleteWithRetry(3);
                    Directory.Delete(_projectDirectory, recursive: true);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Failed to delete {0} directory.", new[] { _projectDirectory });
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        private void DeleteWithRetry(int retries)
        {
            for (var i= 0; i <= retries; i++)
            {
                try
                {
                    Directory.Delete(_projectDirectory, recursive: true);
                    break;
                }
                catch (Exception)
                {
                    if (i < retries)
                    {
                        Debug.WriteLine("Failed to delete {0} directory. Retrying.", _projectDirectory);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}