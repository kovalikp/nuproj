﻿using System.Collections.Generic;
using System.Linq;

namespace NuProj.Tests.Infrastructure
{
    public static class Extensions
    {
        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            return source;
        }
    }
}