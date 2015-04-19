﻿using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace NuProj.Tests.NuGet
{
    public static class Extensions
    {
        public static IEnumerable<string> Flatten(this IEnumerable<PackageDependencySet> dependencySets)
        {
            return from formattedDependency in
                       (from dependencySet in dependencySets
                        from dependency in dependencySet.Dependencies
                        select dependencySet.TargetFramework == null
                        ? dependency.ToString()
                        : string.Format("{0} ({1})", dependency, VersionUtility.GetShortFrameworkName(dependencySet.TargetFramework)))
                   select formattedDependency.Replace("\u2265", ">=").Replace("\u2264", "<=");
        }
    }
}