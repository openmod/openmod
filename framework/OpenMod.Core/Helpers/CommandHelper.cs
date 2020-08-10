using OpenMod.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Core.Helpers
{
    public static class CommandHelper
    {
        public static async Task<object[]> GetServiceOrCommandParameter(
            IEnumerable<Type> types, IServiceProvider serviceProvider, ICommandParameters commandParameters)
        {
            object[] values = new object[types.Count()];
            int commandParamIndex = 0;

            for (int i = 0; i < values.Length; i++)
            {
                Type type = types.ElementAt(i);

                // try get service
                values[i] = serviceProvider.GetService(type);

                // parse command parameter value if the type is not a service
                if (values[i] == null)
                {
                    values[i] = await commandParameters.GetAsync(commandParamIndex++, type);
                }
            }

            return values;
        }
    }
}
