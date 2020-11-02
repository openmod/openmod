using OpenMod.API.Commands;
using OpenMod.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.Parameters
{
    public class TimeSpanCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        public bool Supports(Type type)
        {
            return typeof(TimeSpan) == type;
        }

        public Task<object> ResolveAsync(Type type, string input)
        {
            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            return Task.FromResult((object)TimeSpanHelper.Parse(input));
        }
    }
}
