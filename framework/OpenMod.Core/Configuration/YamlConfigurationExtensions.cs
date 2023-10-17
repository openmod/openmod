// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using OpenMod.Core.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="YamlConfigurationExtensions"/>.
    /// Ex: supports variables.
    /// </summary>
    public static class YamlConfigurationExtensions
    {
        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(this IConfigurationBuilder builder, string path)
        {
            return AddYamlFileEx(builder, provider: null, variables: null, path: path, optional: false, reloadOnChange: false);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddYamlFileEx(builder, provider: null, variables: null, path: path, optional: optional, reloadOnChange: false);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddYamlFileEx(builder, provider: null, variables: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        /// <summary>
        /// Adds the YAML configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="variables">Variables to replace.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(this IConfigurationBuilder builder, string path, IDictionary<string, string> variables, bool optional, bool reloadOnChange)
        {
            return AddYamlFileEx(builder, provider: null, path: path, variables: variables, optional: optional, reloadOnChange: reloadOnChange);
        }

        /// <summary>
        /// Adds a YAML configuration source to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        ///     <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="variables">Variables to replace.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(
            this IConfigurationBuilder builder,
            IFileProvider? provider,
            string path, IDictionary<string, string>? variables,
            bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }

            return builder.AddYamlFileEx(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.Variables = variables;
                s.ResolveFileProvider();
            });
        }

        /// <summary>
        /// Adds a YAML configuration source to <paramref name="builder"/>.
        /// Ex: supports variables.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddYamlFileEx(this IConfigurationBuilder builder, Action<YamlConfigurationSourceEx> configureSource)
            => builder.Add(configureSource);
    }
}