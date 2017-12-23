using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.VS;

namespace NuProj.ProjectSystem
{
    [Export]
    [AppliesTo(NuProjCapabilities.NuProj)]
    [ProjectTypeRegistration(NuProjPackage.ProjectTypeGuid,
                             "NuGet",
                             "#2",
                             NuProjPackage.ProjectExtension,
                             NuProjPackage.ProjectLanguage,
                             NuProjPackage.PackageGuid,
                             PossibleProjectExtensions = NuProjPackage.ProjectExtension,
                             ProjectTemplatesDir = @"ProjectTemplates")]
    internal sealed class NuProjUnconfiguredProject
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; private set; }

        [Import]
        public ActiveConfiguredProject<NuProjConfiguredProject> NuProjActiveConfiguredProject { get; private set; }

        [Import]
        public ConfiguredProject ActiveConfiguredProject
        {
            get { return NuProjActiveConfiguredProject.Value.ConfiguredProject; }
        }
    }
}
