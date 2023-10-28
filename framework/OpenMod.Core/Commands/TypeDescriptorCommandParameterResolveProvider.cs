using System;
using System.ComponentModel;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [Priority(Priority = Priority.Lowest)]
    public class TypeDescriptorCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IOpenModStringLocalizer m_OpenModStringLocalizer;

        public TypeDescriptorCommandParameterResolveProvider(IServiceProvider serviceProvider, IOpenModStringLocalizer openModStringLocalizer)
        {
            m_ServiceProvider = serviceProvider;
            m_OpenModStringLocalizer = openModStringLocalizer;
        }

        public bool Supports(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return TryGetSupportedConverter(type, out _);
        }

        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!TryGetSupportedConverter(type, out TypeConverter converter))
            {
                var ex = new ArgumentException("The given type is not supported", nameof(type));
                return Task.FromException<object?>(ex);
            }

            var descriptor = new ServiceProviderTypeDescriptor(m_ServiceProvider);

            try
            {
                var result = converter.ConvertFromString(descriptor, input);
                return Task.FromResult<object?>(result);
            }
            catch (Exception ex) when (ex.InnerException is OverflowException)
            {
                var parseException = new CommandParameterParseException(m_OpenModStringLocalizer["commands:errors:overflow_error", new { Value = input, Type = type }]!, input, type);
                return Task.FromException<object?>(parseException);
            }
            catch (Exception ex) when (ex.InnerException is FormatException)
            {
                var parseException = new CommandParameterParseException(m_OpenModStringLocalizer["commands:errors:parse_error", new { Value = input, Type = type }]!, input, type);
                return Task.FromException<object?>(parseException);
            }
        }

        private bool TryGetSupportedConverter(Type type, out TypeConverter converter)
        {
            converter = TypeDescriptor.GetConverter(type);
            return converter.CanConvertFrom(typeof(string));
        }
    }
}
