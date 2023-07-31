using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class AsyncLocalCurrentCommandContextAccessor : ICurrentCommandContextAccessor
    {
        private readonly AsyncLocal<ICommandContext?> m_Context = new();

        public ICommandContext? Context
        {
            get { return m_Context.Value; }
            set { m_Context.Value = value; }
        }
    }
}