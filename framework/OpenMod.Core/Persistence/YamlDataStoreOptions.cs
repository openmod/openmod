using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace OpenMod.Core.Persistence
{
    public class YamlDataStoreOptions
    {
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public IReadOnlyList<IYamlTypeConverter> Converters
        {
            get => m_Converters.AsReadOnly();
        }

        private readonly List<IYamlTypeConverter> m_Converters;

        public YamlDataStoreOptions()
        {
            m_Converters = new List<IYamlTypeConverter>();
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
    }
}
