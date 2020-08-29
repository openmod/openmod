using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace OpenMod.Core.Persistence
{
    [OpenModInternal]
    public class YamlDataStore : IDataStore, IDisposable
    {
        [CanBeNull]
        private readonly string m_Prefix;
        private readonly string m_BasePath;

        [CanBeNull]
        private readonly string m_Suffix;
        private readonly ISerializer m_Serializer;
        private readonly IDeserializer m_Deserializer;
        private readonly List<KeyValuePair<IOpenModComponent, Action>> m_ChangeListeners;
        private FileSystemWatcher m_FileSystemWatcher;
        private static ConcurrentDictionary<string, object> s_Locks;

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
                .EmitDefaults()
                .Build();

            m_Deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            m_ChangeListeners = new List<KeyValuePair<IOpenModComponent, Action>>();
            s_Locks = new ConcurrentDictionary<string, object>();
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

        public IDisposable AddChangeWatcher(string key, IOpenModComponent component, Action onChange)
        {
            var filePath = GetFilePathForKey(key);

            var directory = Path.GetDirectoryName(filePath);

            if (directory == null) throw new Exception("Unable to retrieve directory info for file");

            var fileName = Path.GetFileName(filePath);

            lock (GetLock(filePath))
            {
                m_ChangeListeners.Add(new KeyValuePair<IOpenModComponent, Action>(component, onChange));
                var idx = m_ChangeListeners.Count - 1;

                if (idx == 0)
                {
                    // first element, start watcher
                    m_FileSystemWatcher = new FileSystemWatcher(directory, fileName);
                    m_FileSystemWatcher.Changed += (s, a) => OnFileChange(filePath);
                }

                return new DisposeAction(() =>
                {
                    lock (GetLock(filePath))
                    {
                        m_ChangeListeners.RemoveAt(idx);

                        if (m_ChangeListeners.Count == 0)
                        {
                            m_FileSystemWatcher.Dispose();
                            m_FileSystemWatcher = null;
                        }
                    }
                });
            }
        }

        private void OnFileChange(string filePath)
        {
            foreach (var listener in m_ChangeListeners)
            {
                if (!listener.Key.IsComponentAlive)
                {
                    continue;
                }

                listener.Value?.Invoke();
            }
        }

        private object GetLock(string filePath)
        {
            if (!s_Locks.TryGetValue(filePath, out var @lock))
            {
                @lock = new object();
                s_Locks.TryAdd(filePath, @lock);
            }

            return @lock;
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

        public void Dispose()
        {
            m_FileSystemWatcher?.Dispose();
        }
    }
}