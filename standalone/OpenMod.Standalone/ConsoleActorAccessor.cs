using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Console;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Console;

namespace OpenMod.Standalone
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ConsoleActorAccessor : IConsoleActorAccessor
    {
        public ConsoleActorAccessor(IServiceProvider serviceProvider)
        {
            Actor = ActivatorUtilities.CreateInstance<ConsoleActor>(serviceProvider, "openmod-standalone-console", true);
        }

        public IConsoleActor Actor { get; } 
    }
}