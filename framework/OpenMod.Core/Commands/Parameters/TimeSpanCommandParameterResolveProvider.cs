using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.Parameters
{
    [Priority(Priority = Priority.Low)]
    public class TimeSpanCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        public bool Supports(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof(TimeSpan) == type;
        }

        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            return Task.FromResult((object?)TimeSpanHelper.Parse(input));
        }
    }
}
