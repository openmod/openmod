using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Persistence;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Persistence
{
    public class YamlDataStore : IDataStore
    {
        [CanBeNull]
        private readonly string m_Prefix;
        private readonly string m_BasePath;
        
        [CanBeNull]
        private readonly string m_Suffix;
        private readonly Serializer m_Serializer;
        private readonly Deserializer m_Deserializer;

        public YamlDataStore([CanBeNull] string prefix, string basePath, string suffix = "data")
        {
            suffix ??= string.Empty;
            if (!string.IsNullOrEmpty(prefix))
            {
                m_Prefix = $"{prefix}.";
            }

            m_BasePath = basePath;

            if (!string.IsNullOrEmpty(m_Suffix))
            {
                m_Suffix = $".{suffix}";
            }

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

            File.WriteAllBytes(filePath, encodedData);
            return Task.CompletedTask;

            //bug: the follow lines work on .NET Core / Framework but not on mono 
            //using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            //return fileStream.WriteAsync(encodedData, 0, encodedData.Length);
        }

        public Task<bool> ExistsAsync(string key)
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            return Task.FromResult(File.Exists(filePath));
        }

        public virtual Task<T> LoadAsync<T>(string key) where T : class
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<T>(default);
            }

            // see SaveAsync for why this is commented
            //using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
            //var encodedData = new byte[stream.Length];
            //await stream.ReadAsync(encodedData, 0, (int)stream.Length);

            var encodedData = File.ReadAllBytes(filePath);
            var serializedYaml = Encoding.UTF8.GetString(encodedData);
            return Task.FromResult(m_Deserializer.Deserialize<T>(serializedYaml));
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
            return Path.Combine(m_BasePath, $"{m_Prefix ?? string.Empty}{key}{m_Suffix ?? string.Empty}.yaml");
        }
    }
}