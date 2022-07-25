using System;
using System.Runtime.CompilerServices;
using SmartFormat;
using SmartFormat.Extensions;

namespace OpenMod.Core.Helpers
{
    public static class SmartFormatterHelper
    {
        // https://github.com/axuno/SmartFormat/wiki/Async-and-Thread-Safety
        [ThreadStatic]
        private static SmartFormatter? s_SmartFormatter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SmartFormatter ObtainSmartFormatter()
        {
            return s_SmartFormatter ??= Smart.CreateDefaultSmartFormat().AddExtensions(new TimeFormatter());
        }
    }
}
