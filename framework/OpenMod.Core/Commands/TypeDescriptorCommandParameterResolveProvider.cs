using System;
using System.ComponentModel;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [Priority(Priority = Priority.Lowest)]
    public class TypeDescriptorCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IServiceProvider m_ServiceProvider;

        public TypeDescriptorCommandParameterResolveProvider(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        public bool Supports(Type type)
        {
            return TryGetSupportedConverter(type, out _);
        }

        public Task<object> ResolveAsync(Type type, string input)
        {
            if (!TryGetSupportedConverter(type, out TypeConverter converter))
            {
                var ex = new ArgumentException("The given type is not supported", nameof(type));
                return Task.FromException<object>(ex);
            }

            var descriptor = new ServiceProviderTypeDescriptor(m_ServiceProvider);
            object result = converter.ConvertFromString(descriptor, input);

            return Task.FromResult(result);
        }

        private bool TryGetSupportedConverter(Type type, out TypeConverter converter)
        {
            converter = TypeDescriptor.GetConverter(type);
            return converter != null && converter.CanConvertFrom(typeof(string));
        }
    }
}
