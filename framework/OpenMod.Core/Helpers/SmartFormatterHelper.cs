using System;
using System.Runtime.CompilerServices;
using OpenMod.API;
using OpenMod.Core.Localization;
using SmartFormat;
using SmartFormat.Extensions;

namespace OpenMod.Core.Helpers
{
    [Obsolete("This class will be set to internal in future releases. To get SmartFormatter use " + nameof(SmartFormatOptions.GetSmartFormatter))]
    [OpenModInternal]
    public static class SmartFormatterHelper
    {
        // https://github.com/axuno/SmartFormat/wiki/Async-and-Thread-Safety
        // since v3.1.0 Smart.Default is flagged with [ThreadStatic], but it creates the default formatter and it's not possible to override it.
        // So we are storing our formatter here
        [ThreadStatic]
        private static SmartFormatter? s_SmartFormatter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Use " + nameof(SmartFormatOptions.GetSmartFormatter) + " to get SmartFormatter")]
        public static SmartFormatter ObtainSmartFormatter()
        {
            return s_SmartFormatter ??= Smart.CreateDefaultSmartFormat().AddExtensions(new TimeFormatter());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SmartFormatter ObtainSmartFormatter(SmartFormatOptions options)
        {
            return s_SmartFormatter ??= options.CreateSmartFormatter();
        }
    }
}
