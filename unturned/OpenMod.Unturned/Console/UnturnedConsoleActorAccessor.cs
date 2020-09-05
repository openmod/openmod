using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Console;
using System;

namespace OpenMod.Unturned.Console
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UnturnedConsoleActorAccessor : IConsoleActorAccessor
    {
        public UnturnedConsoleActorAccessor(IServiceProvider serviceProvider)
        {
            Actor = ActivatorUtilities.CreateInstance<ConsoleActor>(serviceProvider, "openmod-unturned-console");
        }

        public ICommandActor Actor { get; }
    }
}