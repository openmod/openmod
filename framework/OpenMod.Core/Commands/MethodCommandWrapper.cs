using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Commands;
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

        public MethodCommandWrapper(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            m_MethodInfo = methodInfo;
            m_ServiceProvider = serviceProvider;
        }

        public async Task ExecuteAsync()
        {
            IEnumerable<Type> paramTypes = m_MethodInfo.GetParameters().Select(x => x.ParameterType);
            object[] paramValues = paramTypes.Select(x => m_ServiceProvider.GetService(x)).ToArray();

            await m_MethodInfo.InvokeWithTaskSupportAsync(null, paramValues);
        }
    }
}