﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenMod.Common.Helpers
{
    /// <summary>
    /// Comparer of <see cref="AssemblyName"/> that comparers the name
    /// </summary>
    public sealed class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
    {
        public static AssemblyNameEqualityComparer Instance { get; } = new();

        private AssemblyNameEqualityComparer() { }

        public bool Equals(AssemblyName? x, AssemblyName? y)
        {
            return string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(AssemblyName obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
        }
    }
}
