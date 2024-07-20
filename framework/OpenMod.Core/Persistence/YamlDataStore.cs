﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Persistence
{
    [OpenModInternal]
    public class YamlDataStore : IDataStore, IDisposable
    {
        private readonly string? m_Prefix;
        private readonly string? m_Suffix;
        private readonly string m_BasePath;

        private readonly List<RegisteredChangeListener> m_ChangeListeners;
        private readonly ILogger<YamlDataStore>? m_Logger;
        private readonly IOpenModSerializer m_Serializer;
        private readonly IRuntime? m_Runtime;
        private readonly List<string> m_WatchedFiles;
        private readonly ConcurrentDictionary<string, object> m_Locks;
        private readonly bool m_LogOnChange;
        private readonly Dictionary<string, int> m_WriteCounter;
        private readonly HashSet<string> m_KnownKeys;
        private readonly Dictionary<string, string> m_ContentHash;
        private FileSystemWatcher? m_FileSystemWatcher;

        public YamlDataStore(DataStoreCreationParameters parameters,
            ILogger<YamlDataStore>? logger,
            IOpenModSerializer serializer,
            IRuntime? runtime)
        {
            m_LogOnChange = parameters.LogOnChange;
            m_Logger = logger;
            m_Serializer = serializer;
            m_Runtime = runtime;
            m_WriteCounter = [];
            m_WatchedFiles = [];
            m_KnownKeys = [];
            m_ContentHash = [];

            if (!string.IsNullOrEmpty(parameters.Prefix))
            {
                m_Prefix = $"{parameters.Prefix}.";
            }

            m_BasePath = parameters.WorkingDirectory;

            if (!string.IsNullOrEmpty(parameters.Suffix))
            {
                m_Suffix = $".{parameters.Suffix}";
            }

            m_ChangeListeners = [];
            m_Locks = new ConcurrentDictionary<string, object>();

            EnsureFileSystemWatcherCreated(false);
        }

        private void EnsureFileSystemWatcherCreated(bool createDirectory)
        {
            if (m_FileSystemWatcher != null)
            {
                return;
            }

            if (!Directory.Exists(m_BasePath))
            {
                if (!createDirectory)
                {
                    return;
                }

                Directory.CreateDirectory(m_BasePath);
            }

            m_FileSystemWatcher = new FileSystemWatcher(m_BasePath)
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.yaml"
            };

            m_FileSystemWatcher.Changed += (_, args) =>
            {
                AsyncHelper.Schedule(
                    name: GetType().Name + "_" + nameof(ChangeEventHandler),
                    task: () => ChangeEventHandler(args),
                    ex => m_Logger?.LogError(ex, "Error occured on file change for: {FilePath}", args.FullPath));
            };
        }

        private async Task ChangeEventHandler(FileSystemEventArgs a)
        {
            if (a.ChangeType == WatcherChangeTypes.Renamed ||
                a.ChangeType == WatcherChangeTypes.Deleted)
            {
                return;
            }

            foreach (var key in m_KnownKeys)
            {
                var filePath = GetFilePathForKey(key);

                if (!a.FullPath.Equals(Path.GetFullPath(filePath), StringComparison.Ordinal))
                {
                    continue;
                }

                using var sha = new SHA256Managed();
                var encodedData = await Retry.DoAsync(
                    () => File.ReadAllBytes(filePath),
                    retryInterval: TimeSpan.FromMilliseconds(1),
                    maxAttempts: 5);
                var fileHash = BitConverter.ToString(sha.ComputeHash(encodedData));

                if (string.Equals(m_ContentHash[key], fileHash, StringComparison.Ordinal))
                {
                    // Nothing changed
                    return;
                }

                lock (GetLock(key))
                {
                    if (DecrementWriteCounter(key))
                    {
                        m_Logger?.LogDebug("File changed: {FilePath} ({ChangeType:X})", a.FullPath, a.ChangeType);
                        OnFileChange(key);
                    }
                }

                return;
            }
        }

        public virtual async Task SaveAsync<T>(string key, T? data) where T : class
        {
            await SaveAsync(key, data, string.Empty);
        }

        internal async Task SaveAsync<T>(string key, T? data, string header) where T : class
        {
            CheckKeyValid(key);

            var headerData = Encoding.UTF8.GetBytes(header);//exception only if null
            var dataToMemory = data != null ? await m_Serializer.SerializeAsync(data) : ReadOnlyMemory<byte>.Empty;

            byte[] dataToSave;
            await using (var memoryStream = new MemoryStream(headerData.Length + dataToMemory.Length))
            {
                if (headerData.Length > 0)
                {
                    await memoryStream.WriteAsync(headerData);
                }

                await memoryStream.WriteAsync(dataToMemory);
                dataToSave = memoryStream.ToArray();
            }

            var filePath = GetFilePathForKey(key);
            RegisterKnownKey(key);

            lock (GetLock(key))
            {
                using var sha = new SHA256Managed();
                var contentHash = BitConverter.ToString(sha.ComputeHash(dataToSave));

                var directory = Path.GetDirectoryName(filePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                else if (File.Exists(filePath))
                {
                    using var fileStream = File.OpenRead(filePath);
                    var fileHash = BitConverter.ToString(sha.ComputeHash(fileStream));

                    // Ensure that we are actually writing different data
                    // otherwise the file change watcher won't trigger and result in desynced write counter state
                    if (string.Equals(fileHash, contentHash, StringComparison.Ordinal))
                    {
                        return;
                    }
                }

                IncrementWriteCounter(key);

                var wasRaising = false;
                if (m_FileSystemWatcher != null)
                {
                    wasRaising = m_FileSystemWatcher.EnableRaisingEvents;
                    m_FileSystemWatcher.EnableRaisingEvents = false;
                }

                try
                {
                    File.WriteAllBytes(filePath, dataToSave);
                    m_ContentHash[key] = contentHash;
                }
                catch
                {
                    DecrementWriteCounter(key);
                    throw;
                }
                finally
                {
                    if (m_FileSystemWatcher != null)
                    {
                        m_FileSystemWatcher.EnableRaisingEvents = wasRaising;
                    }
                }
            }

            //bug: the follow lines work on .NET Core / Framework but not on mono
            //using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            //return fileStream.WriteAsync(encodedData, 0, encodedData.Length);
        }

        public Task<bool> ExistsAsync(string key)
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            var fileInfo = new FileInfo(filePath);
            return Task.FromResult(fileInfo.Exists && fileInfo.Length > 0);
        }

        public virtual async Task<T?> LoadAsync<T>(string key) where T : class
        {
            CheckKeyValid(key);

            var filePath = GetFilePathForKey(key);
            if (!File.Exists(filePath))
            {
                throw new Exception($"Load called on Yaml file that doesnt exist: {filePath}");
            }

            RegisterKnownKey(key);

            // see SaveAsync for why this is commented
            //using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
            //var encodedData = new byte[stream.Length];
            //await stream.ReadAsync(encodedData, 0, (int)stream.Length);

            // Retry fixes stupid IOException ("File being used by another process")
            // when reading from a FileSystemWatcher's event.
            //
            // Similar: https://stackoverflow.com/q/49551278
            var encodedData = await Retry.DoAsync(() => File.ReadAllBytes(filePath), TimeSpan.FromMilliseconds(1), 5);

            using var sha = new SHA256Managed();
            var contentHash = BitConverter.ToString(sha.ComputeHash(encodedData));
            m_ContentHash[key] = contentHash;

            var serializedYaml = Encoding.UTF8.GetString(encodedData);
            return await m_Serializer.DeserializeAsync<T>(encodedData);
        }

        public IDisposable AddChangeWatcher(string key, IOpenModComponent component, Action onChange)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (onChange == null)
            {
                throw new ArgumentNullException(nameof(onChange));
            }

            CheckKeyValid(key);
            RegisterKnownKey(key);

            if (m_FileSystemWatcher is not null)
            {
                m_FileSystemWatcher.EnableRaisingEvents = true;
            }

            var @lock = GetLock(key);
            lock (@lock)
            {
                var changeListener = new RegisteredChangeListener(component, key, onChange);
                m_ChangeListeners.Add(changeListener);

                return new DisposeAction(() =>
                {
                    lock (@lock)
                    {
                        m_ChangeListeners.Remove(changeListener);
                    }
                });
            }
        }

        private void OnFileChange(string key)
        {
            foreach (var listener in m_ChangeListeners.ToList())
            {
                if (!listener.Key.Equals(key, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!listener.Component.IsComponentAlive)
                {
                    continue;
                }

                listener.Callback.Invoke();
            }
        }

        private object GetLock(string key)
        {
            if (!m_Locks.TryGetValue(key, out var @lock))
            {
                @lock = new object();
                m_Locks.TryAdd(key, @lock);
            }

            return @lock;
        }

        protected virtual void CheckKeyValid(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception($"Invalid data store key: {key}. Key cannot be null or empty.");
            }

            if (!char.IsLetter(key[0]))
            {
                throw new Exception($"Invalid data store key: {key}. Key must begin with a letter.");
            }

            if (!key.All(d => char.IsLetterOrDigit(d) || d == '.'))
            {
                throw new Exception(
                    $"Invalid data store key: {key}. Key can only consist of alphanumeric characters and dot");
            }
        }

        private void RegisterKnownKey(string key)
        {
            EnsureFileSystemWatcherCreated(true);

            // Will add a change watcher that logs detected file changes
            m_KnownKeys.Add(key);

            if (!m_LogOnChange || m_WatchedFiles.Contains(key) || m_Runtime == null)
            {
                return;
            }

            m_WatchedFiles.Add(key);
            AddChangeWatcher(key, m_Runtime!, () =>
            {
                m_Logger?.LogInformation("Reloaded {Prefix}{Key}{Suffix}.yaml",
                    m_Prefix ?? string.Empty, key, m_Suffix ?? string.Empty);
            });
        }

        protected virtual string GetFilePathForKey(string key)
        {
            return Path.Combine(m_BasePath, $"{m_Prefix ?? string.Empty}{key}{m_Suffix ?? string.Empty}.yaml");
        }

        private void IncrementWriteCounter(string key)
        {
            if (!m_WriteCounter.TryAdd(key, 1))
            {
                m_WriteCounter[key]++;
            }
        }

        private bool DecrementWriteCounter(string key)
        {
            if (!m_WriteCounter.ContainsKey(key))
            {
                return true;
            }

#pragma warning disable IDE0066 // Convert switch to expression
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch (m_WriteCounter[key])
            {
                case 0:
                    return true;
                case < 0:
                    // race condition? can only happen if called outside lock
                    throw new InvalidOperationException("DecrementWriteCounter has become negative");
                default:
                    return m_WriteCounter[key]-- == 0;
            }
#pragma warning restore IDE0066 // Convert switch to expression
        }

        public void Dispose()
        {
            m_WriteCounter.Clear();
            m_WatchedFiles.Clear();
            m_ChangeListeners.Clear();
            m_FileSystemWatcher?.Dispose();
            m_Locks.Clear();
            m_ContentHash.Clear();
        }

        private class RegisteredChangeListener(IOpenModComponent component, string key, Action callback)
        {
            public IOpenModComponent Component { get => component; }

            public string Key { get => key; }

            public Action Callback { get => callback; }
        }
    }
}