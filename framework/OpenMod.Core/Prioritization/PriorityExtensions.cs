using System;
using System.Reflection;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Prioritization
{
    public static class PriorityExtensions
    {
        public static Priority GetPriority(this Type t)
        {
            // I am not sure, but I don't think GetCustomAttribute<PriorityAttribute>
            // would return derived PriorityAttribute such as EventListenerAttribute
            
            var priorityAttribute = new PriorityAttribute();
            foreach (var attribute in t.GetCustomAttributes())
            {
                if (attribute is PriorityAttribute priorityAttribute2)
                {
                    priorityAttribute = priorityAttribute2;
                    break;
                }
            }

            return priorityAttribute.Priority;
        }
    }
}