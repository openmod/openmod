using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.API.Commands
{
    public interface ICommandParameterResolveProvider
    {
        /// <summary>
        /// Determines whether the given type is supported by this resolver.
        /// </summary>
        bool Supports([NotNull] Type type);

        /// <summary>
        /// Resolves an object of the given type from the input.
        /// </summary>
        [ItemCanBeNull]
        Task<object> ResolveAsync([NotNull] Type type, [NotNull] string input);
    }
}
