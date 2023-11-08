using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nito.Disposables.Internals;
using OpenMod.Core.Helpers;
using SmartFormat;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Settings;
using SmartFormat.Utilities;

namespace OpenMod.Core.Localization;
public class SmartFormatOptions
{
    private readonly List<FormatterFactory<IFormatter>> m_Formatters;
    private readonly List<FormatterFactory<ISource>> m_Sources;
    private readonly SmartSettings m_SmartSettings;

    // used to check if formatter already initialized,
    // if so, it throws an exception when trying to modify options
    private bool m_IsInitialized;

    public SmartFormatOptions()
    {
        m_SmartSettings = new SmartSettings();

        var defaultFormatter = Smart.Default;

        // copy formatters and sources
        m_Formatters = new();
        foreach (var formatter in defaultFormatter.GetFormatterExtensions())
        {
            var formatterType = formatter.GetType();

            m_Formatters.Add(new(formatterType, factory =>
            {
                try
                {
                    return Activator.CreateInstance(factory.Type) as IFormatter;
                }
                catch
                {
                    return null;
                }
            }));
        }

        m_Sources = new();
        foreach (var source in defaultFormatter.GetSourceExtensions())
        {
            var sourceType = source.GetType();

            m_Sources.Add(new(sourceType, factory =>
            {
                try
                {
                    return Activator.CreateInstance(factory.Type) as ISource;
                }
                catch
                {
                    return null;
                }
            }));
        }

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
    /// <returns><see langword="true"/>, if the extension was added.</returns>
    public bool TryAddFormatter<T>() where T : class, IFormatter, new()
    {
        CheckInitialized();

        if (m_Formatters.Any(x => x.Type == typeof(T)))
        {
            return false;
        }

        m_Formatters.Add(new(typeof(T), _ => new T()));
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

        var index = m_Formatters.FindIndex(f => f.Type == typeof(T));
        if (index == -1)
        {
            return false;
        }

        m_Formatters.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Adds <see cref="ISource"/> extension to the source list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns><see langword="true"/>, if the extension was added.</returns>
    public bool TryAddSource<T>() where T : class, ISource, new()
    {
        CheckInitialized();

        if (m_Sources.Any(x => x.Type == typeof(T)))
        {
            return false;
        }

        m_Sources.Add(new(typeof(T), _ => new T()));
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

        var index = m_Sources.FindIndex(f => f.Type == typeof(T));
        if (index == -1)
        {
            return false;
        }

        m_Sources.RemoveAt(index);
        return true;
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
        return SmartFormatterHelper.ObtainSmartFormatter(this);
    }

    internal SmartFormatter CreateSmartFormatter()
    {
        m_IsInitialized = true;

        return new SmartFormatter(m_SmartSettings)
            .AddExtensions(m_Sources.Select(x => x.Factory(x))
                .WhereNotNull()
                .ToArray())
            .AddExtensions(m_Formatters.Select(x => x.Factory(x))
                .WhereNotNull()
                .ToArray());
    }

    private sealed class FormatterFactory<T>
    {
        public Func<FormatterFactory<T>, T?> Factory { get; }
        public Type Type { get; }

        public FormatterFactory(Type type, Func<FormatterFactory<T>, T?> factory)
        {
            Factory = factory;
            Type = type;
        }
    }
}
