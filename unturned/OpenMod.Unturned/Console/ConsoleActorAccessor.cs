using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Console;

namespace OpenMod.Unturned.Console
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ConsoleActorAccessor : IConsoleActorAccessor
    {
        public ConsoleActorAccessor(IServiceProvider serviceProvider)
        {
            Actor = ActivatorUtilities.CreateInstance<ConsoleActor>(serviceProvider, "openmod-unturned-console");
        }

        public ICommandActor Actor { get; } 
    }
}