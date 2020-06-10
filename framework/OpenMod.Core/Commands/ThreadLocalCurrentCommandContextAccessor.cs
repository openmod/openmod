using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ThreadLocalCurrentCommandContextAccessor : ICurrentCommandContextAccessor
    {
        private readonly ThreadLocal<ICommandContext> m_Context = new ThreadLocal<ICommandContext>(() => null);

        public ICommandContext Context
        {
            get { return m_Context.Value; }
            set { m_Context.Value = value; }
        }
    }
}