using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// The service for resolving command parameters.
    /// </summary>
    [Service]
    public interface ICommandParameterResolver
    {
        /// <summary>
        /// Resolves an object of the given type from the input.
        /// </summary>
        [ItemCanBeNull]
        Task<object> ResolveAsync([NotNull] Type type, [NotNull] string input);
    }
}
