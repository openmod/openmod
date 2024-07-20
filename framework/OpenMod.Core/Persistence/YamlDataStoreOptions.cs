using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VYaml.Serialization;
using YamlDotNet.Serialization;

namespace OpenMod.Core.Persistence
{
    public class YamlDataStoreOptions
    {
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        private readonly List<IYamlTypeConverter> m_Converters;
        public IReadOnlyList<IYamlTypeConverter> Converters
        {
            get => m_Converters.AsReadOnly();
        }

        private readonly List<IYamlFormatter> m_Formatters;
        public IReadOnlyList<IYamlFormatter> Formatters
        {
            get => m_Formatters.AsReadOnly();
        }

        private readonly List<IYamlFormatterResolver> m_FormatterResolvers;
        public IReadOnlyList<IYamlFormatterResolver> FormatterResolvers
        {
            get => m_FormatterResolvers.AsReadOnly();
        }

        public YamlDataStoreOptions()
        {
            m_Converters = [];
            m_Formatters = [];
            m_FormatterResolvers = [];
        }

        /// <summary>
        /// Adds the specified <see cref="IYamlTypeConverter"/> to the converters list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is added to converters list; <see langword="false"/> if the converter is already present.</returns>
        public bool TryAddConverter<T>() where T : class, IYamlTypeConverter, new()
        {
            if (m_Converters.Any(c => c.GetType() == typeof(T)))
            {
                return false;
            }

            m_Converters.Add(new T());
            return true;
        }

        /// <summary>
        /// Removes the specified <see cref="IYamlTypeConverter"/> from the converters list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        [UsedImplicitly]
        public bool TryRemoveConverter<T>() where T : class, IYamlTypeConverter, new()
        {
            var indexOfConverter = m_Converters.FindIndex(c => c.GetType() == typeof(T));

            if (indexOfConverter == -1)
            {
                return false;
            }

            m_Converters.RemoveAt(indexOfConverter);
            return true;
        }

        /// <summary>
        /// Adds the specified <see cref="IYamlFormatter"/> to the formatters list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is added to converters list; <see langword="false"/> if the converter is already present.</returns>
        public bool TryAddFormatter<T>() where T : class, IYamlFormatter, new()
        {
            if (m_Formatters.Any(c => c.GetType() == typeof(T)))
            {
                return false;
            }

            m_Formatters.Add(new T());
            return true;
        }

        /// <summary>
        /// Removes the specified <see cref="IYamlFormatter"/> from the formatters list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        [UsedImplicitly]
        public bool TryRemoveFormatter<T>() where T : class, IYamlFormatter, new()
        {
            var indexOfConverter = m_Formatters.FindIndex(c => c.GetType() == typeof(T));
            if (indexOfConverter == -1)
            {
                return false;
            }

            m_Formatters.RemoveAt(indexOfConverter);
            return true;
        }

        /// <summary>
        /// Adds the specified <see cref="IYamlFormatterResolver"/> to the formatter resolvers list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is added to converters list; <see langword="false"/> if the converter is already present.</returns>
        public bool TryAddFormatterResolver<T>() where T : class, IYamlFormatterResolver, new()
        {
            if (m_FormatterResolvers.Any(c => c.GetType() == typeof(T)))
            {
                return false;
            }

            m_FormatterResolvers.Add(new T());
            return true;
        }

        /// <summary>
        /// Removes the specified <see cref="IYamlFormatterResolver"/> from the formatter resolvers list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see langword="true"/> if the converter is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        [UsedImplicitly]
        public bool TryRemoveFormatterResolver<T>() where T : class, IYamlFormatterResolver, new()
        {
            var indexOfConverter = m_FormatterResolvers.FindIndex(c => c.GetType() == typeof(T));
            if (indexOfConverter == -1)
            {
                return false;
            }

            m_FormatterResolvers.RemoveAt(indexOfConverter);
            return true;
        }
    }
}
