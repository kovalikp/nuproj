using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.ProjectSystem;

namespace NuProj.ProjectSystem
{
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(NuProjCapabilities.NuProj)]
    internal sealed class NuProjProjectTreeModifier
        : IProjectTreePropertiesProvider
    {
        [Import]
        public UnconfiguredProject UnconfiguredProject { get; set; }

        public void CalculatePropertyValues(IProjectTreeCustomizablePropertyContext propertyContext, IProjectTreeCustomizablePropertyValues propertyValues)
        {
            if (propertyValues != null)
            {
                if (propertyValues.Flags.Contains(ProjectTreeFlags.Common.ProjectRoot))
                {
                    propertyValues.Icon = KnownMonikers.NuGet.ToProjectSystemType();
                }
            }
        }
    }
}
