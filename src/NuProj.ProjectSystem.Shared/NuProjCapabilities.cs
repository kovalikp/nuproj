using System;
using Microsoft.VisualStudio.ProjectSystem;

using System.Collections.Immutable;

namespace NuProj.ProjectSystem
{
    internal static class NuProjCapabilities
    {
        public const string NuProj = "NuProj";

        public static readonly ImmutableHashSet<string> ProjectSystem = Empty.CapabilitiesSet.Union(new[]
        {
            NuProj,
            ProjectCapabilities.ProjectConfigurationsDeclaredAsItems,
            ProjectCapabilities.ReferencesFolder,
        });
    }
}
