using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.Core.Helpers;
using SmartFormat;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Settings;
using SmartFormat.Utilities;

namespace OpenMod.Core.Localization;
public class SmartFormatOptions
{
    private readonly List<IFormatter> m_Formatters;
    private readonly List<ISource> m_Sources;
    private readonly SmartSettings m_SmartSettings;

    // used to check if formatter already initialized,
    // if so, it throws an exception when trying to modify options
    private bool m_IsInitialized;

    public SmartFormatOptions()
    {
        m_SmartSettings = new SmartSettings();

        var defaultFormatter = Smart.Default;

        // copy formatters and sources
        m_Formatters = defaultFormatter.GetFormatterExtensions().ToList();
        m_Sources = defaultFormatter.GetSourceExtensions().ToList();

        // Smartformatter will be used in threading, so make sure it is thread safe mode is set
        // more info: https://github.com/axuno/SmartFormat/wiki/Async-and-Thread-Safety
        SmartSettings.IsThreadSafeMode = true;

        // adds support for custom extensions, but removes the string.Format compatibility
        m_SmartSettings.StringFormatCompatibility = false;
    }

    /// <summary>
    /// Adds <see cref="IFormatter"/> extension to the formatters list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="formatter"><see cref="IFormatter"/> instance</param>
    /// <returns><see langword="true"/>, if the extension was added.</returns>
    public bool TryAddFormatter<T>(T formatter) where T : class, IFormatter
    {
        CheckInitialized();

        if (m_Formatters.OfType<T>().FirstOrDefault() is not null)
        {
            return false;
        }

        m_Formatters.Add(formatter);
        return true;
    }

    /// <summary>
    /// Removes <see cref="IFormatter"/> extension from the formatters list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><see langword="true"/>, if the extension was removed.</returns>
    public bool TryRemoveFormatter<T>() where T : class, IFormatter
    {
        CheckInitialized();

        return m_Formatters.RemoveAll(f => f is T) > 0;
    }

    /// <summary>
    /// Adds <see cref="ISource"/> extension to the source list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"><see cref="ISource"/> instance</param>
    /// <returns><see langword="true"/>, if the extension was added.</returns>
    public bool TryAddSource<T>(T source) where T : class, ISource
    {
        CheckInitialized();

        if (m_Sources.OfType<T>().FirstOrDefault() is not null)
        {
            return false;
        }

        m_Sources.Add(source);
        return true;
    }

    /// <summary>
    /// Removes <see cref="ISource"/> extension from the source list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><see langword="true"/>, if the extension was removed.</returns>
    public bool TryRemoveSource<T>() where T : class, ISource
    {
        CheckInitialized();

        return m_Sources.RemoveAll(f => f is T) > 0;
    }

    public void SetLocalizationProvider(ILocalizationProvider? localizationProvider)
    {
        CheckInitialized();

        m_SmartSettings.Localization.LocalizationProvider = localizationProvider;
    }

    private void CheckInitialized()
    {
        if (m_IsInitialized)
        {
            throw new InvalidOperationException("SmartFormatter already initialized");
        }
    }

    public SmartFormatter GetSmartFormatter()
    {
#pragma warning disable CS0618 // Obsolete for other developers that uses this helper
        return SmartFormatterHelper.ObtainSmartFormatter(this);
#pragma warning restore CS0618
    }

    internal SmartFormatter CreateSmartFormatter()
    {
        m_IsInitialized = true;

        return new SmartFormatter(m_SmartSettings)
            .AddExtensions(m_Sources.ToArray())
            .AddExtensions(m_Formatters.ToArray());
    }
}
