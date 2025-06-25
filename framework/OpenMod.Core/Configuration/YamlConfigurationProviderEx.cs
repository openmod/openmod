using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenMod.API;
using OpenMod.Common.Helpers;
using VYaml.Serialization;

namespace OpenMod.Core.Configuration
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationProvider"/>.
    /// Ex: Supports variables.
    /// </summary>
    [OpenModInternal]
    public class YamlConfigurationProviderEx(YamlConfigurationSourceEx source) : FileConfigurationProvider(source)
    {
        public override void Load(Stream stream)
        {
            var binData = stream.ReadAllBytes();
            var decodedData = UTF8Encoding.UTF8.GetString(binData);
            PreProcessYaml(ref decodedData);
            binData = UTF8Encoding.UTF8.GetBytes(decodedData);

            try
            {
                var nodes = YamlSerializer.Deserialize<IDictionary<string, object>>(binData);
                var processedNode = GetKeyValuesFromNode(nodes);
                Data = processedNode.ToImmutableSortedDictionary();
            }
            catch (Exception e)
            {
                throw new FormatException($"Could not parse the YAML file: {e.Message}.", e);
            }
        }

        private IEnumerable<KeyValuePair<string, string?>> GetKeyValuesFromNode(IEnumerable<KeyValuePair<string, object>> nodes)
        {
            foreach (var node in nodes)
            {
                var nodeData = GetKeyValuesFromNode(node.Value);
                foreach (var item in nodeData)
                {
                    yield return KeyValuePair.Create($"{node.Key}:{item.Key}", item.Value);
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string?>> GetKeyValuesFromNode(object node)
        {
            switch (node)
            {
                case IDictionary<object, object> dict:
                    foreach (var item in GetKeyValuesFromNode(dict))
                    {
                        yield return item;
                    }
                    break;

                case IList<object> list:
                    foreach (var item in GetKeyValuesFromNode(list))
                    {
                        yield return item;
                    }
                    break;

                default:
                    var v = node.ToString();
                    //YamlDotNet set all bool to lower case
                    if (node is bool)
                    {
                        v = v.ToLower();
                    }

                    yield return KeyValuePair.Create<string, string?>(string.Empty, v);
                    break;
            }
        }

        private IEnumerable<KeyValuePair<string, string?>> GetKeyValuesFromNode(IDictionary<object, object> node)
        {
            var strBuilder = new StringBuilder();
            foreach (var nodeItem in node)
            {
                var itemData = GetKeyValuesFromNode(nodeItem.Value);
                foreach (var entry in itemData)
                {
                    strBuilder.Clear();
                    strBuilder.Append(nodeItem.Key);
                    if (!string.IsNullOrEmpty(entry.Key))
                    {
                        strBuilder.Append(":");
                        strBuilder.Append(entry.Key);
                    }

                    yield return KeyValuePair.Create(strBuilder.ToString(), entry.Value);
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string?>> GetKeyValuesFromNode(IList<object> node)
        {
            var strBuilder = new StringBuilder();
            for (int i = 0; i < node.Count; i++)
            {
                var item = node[i];
                var itemData = GetKeyValuesFromNode(item);
                foreach (var entry in itemData)
                {
                    strBuilder.Clear();
                    strBuilder.Append(i);
                    if (!string.IsNullOrEmpty(entry.Key))
                    {
                        strBuilder.Append(":");
                        strBuilder.Append(entry.Key);
                    }
                    yield return KeyValuePair.Create(strBuilder.ToString(), entry.Value);
                }
            }
        }

        private void PreProcessYaml(ref string yaml)
        {
            if (source.Variables == null)
            {
                return;
            }

            foreach (var variable in source.Variables)
            {
                yaml = yaml.Replace("{{" + variable.Key + "}}", variable.Value);
            }
        }
    }
}
