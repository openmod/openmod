using System;
using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// A provider for resolving command parameters.
    /// </summary>
    public interface ICommandParameterResolveProvider
    {
        /// <summary>
        /// Determines whether the given type is supported by this resolver.
        /// </summary>
        bool Supports(Type type);

        /// <summary>
        /// Resolves an object of the given type from the input.
        /// </summary>
        Task<object?> ResolveAsync(Type type, string input);
    }
}
