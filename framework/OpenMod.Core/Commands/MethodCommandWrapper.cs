using System;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    [OpenModInternal]
    public class MethodCommandWrapper : ICommand
    {
        private readonly MethodInfo m_MethodInfo;

        public MethodCommandWrapper(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            m_MethodInfo = methodInfo;
        }
        public async Task ExecuteAsync()
        {
            // todo: implement MethodCommandWrapper
            throw new NotImplementedException();
        }
    }
}