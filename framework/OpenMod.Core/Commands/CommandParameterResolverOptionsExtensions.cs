using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public static class CommandParameterResolverOptionsExtensions
    {
        public static void AddCommandParameterResolveProvider<TResolver>(this CommandParameterResolverOptions options)
            where TResolver : ICommandParameterResolveProvider
        {
            options.AddCommandParameterResolveProvider(typeof(TResolver));
        }

        public static bool RemoveCommandParameterResolveProvider<TResolver>(this CommandParameterResolverOptions options)
            where TResolver : ICommandParameterResolveProvider
        {
            return options.RemoveCommandParameterResolveProvider(typeof(TResolver));
        }
    }
}
