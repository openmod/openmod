using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Standalone
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ConsoleActorAccessor : IConsoleActorAccessor
    {
        public ConsoleActor Actor { get; } = new ConsoleActor();
    }
}