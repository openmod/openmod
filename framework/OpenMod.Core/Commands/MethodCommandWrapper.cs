using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    [OpenModInternal]
    public class MethodCommandWrapper : ICommand
    {
        private readonly MethodInfo m_MethodInfo;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ICommandParameters m_CommandParameters;
        private readonly IEnumerable<Type> m_ParametersTypes;

        public MethodCommandWrapper(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            m_MethodInfo = methodInfo;
            m_ServiceProvider = serviceProvider;
            m_CommandParameters = serviceProvider.GetRequiredService<ICurrentCommandContextAccessor>().Context.Parameters;
            m_ParametersTypes = m_MethodInfo.GetParameters().GetParametersTypes();
        }

        public async Task ExecuteAsync()
        {
            object[] values = await CommandHelper.GetServiceOrCommandParameter(m_ParametersTypes, m_ServiceProvider, m_CommandParameters);
            await m_MethodInfo.InvokeWithTaskSupportAsync(null, values);
        }
    }
}
