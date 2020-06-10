using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMod.API.Persistence;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Persistence
{
    public class YamlDataStore : IDataStore
    {
        private readonly string m_BasePath;
        private readonly string m_Suffix;
        private readonly Serializer m_Serializer;
        private readonly Deserializer m_Deserializer;

        public YamlDataStore(string basePath, string suffix = ".data")
        {
            suffix ??= string.Empty;
            m_BasePath = basePath;
            m_Suffix = suffix;
            m_Serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            m_Deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
        }

        public virtual Task SaveAsync<T>(string key, T data) where T : class
        {
            CheckKeyValid(key);
            
            var serializedYaml = m_Serializer.Serialize(data);
            var encodedData = Encoding.UTF8.GetBytes(serializedYaml);
            var filePath = GetFilePathForKey(key);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            return fileStream.WriteAsync(encodedData, 0, encodedData.Length);
        }

        public Task<bool> ExistsAsync(string key)
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            return Task.FromResult(File.Exists(filePath));
        }

        public virtual async Task<T> LoadAsync<T>(string key) where T : class
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            if (!File.Exists(filePath))
            {
                return default;
            }

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
            var encodedData = new byte[stream.Length];
            await stream.ReadAsync(encodedData, 0, (int)stream.Length);
            var serializedYaml = Encoding.UTF8.GetString(encodedData);
            return m_Deserializer.Deserialize<T>(serializedYaml);
        }

        protected virtual void CheckKeyValid(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception($"Invalid data store key: {key}. Key can not be null or empty.");
            }

            if (!char.IsLetter(key[0]))
            {
                throw new Exception($"Invalid data store key: {key}. Key must begin with a letter.");
            }

            if (!key.All(d => char.IsLetterOrDigit(d) || d == '.'))
            {
                throw new Exception($"Invalid data store key: {key}. Key can only consist of alphanumeric characters and dot");
            }
        }

        protected virtual string GetFilePathForKey(string key)
        {
            return Path.Combine(m_BasePath, $"{key}{m_Suffix}.yml");
        }
    }
}