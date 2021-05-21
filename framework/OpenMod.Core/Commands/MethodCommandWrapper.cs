using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
            m_MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            m_ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            var contextAccessor = serviceProvider.GetRequiredService<ICurrentCommandContextAccessor>();

            var context = contextAccessor.Context;
            if (context == null)
            {
                throw new InvalidOperationException("contextAccessor.Context was null");
            }

            m_CommandParameters = context.Parameters;
            m_ParametersTypes = m_MethodInfo.GetParameters().GetParametersTypes();
        }

        public async Task ExecuteAsync()
        {
            object[] values = await CommandHelper.GetServiceOrCommandParameter(
                m_ParametersTypes, m_ServiceProvider, m_CommandParameters);

            try
            {
                await m_MethodInfo.InvokeWithTaskSupportAsync(instance: null, values);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                // Rethrow without losing stack trace
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }
    }
}
