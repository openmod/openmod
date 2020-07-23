using System;
using System.ComponentModel;
using System.Threading;
using OpenMod.API;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class TypeConverterHelper
    {
        public static TypeConverter GetConverter(Type type)
        {
            //todo ??
            //if (typeof(IPlayer).IsAssignableFrom(type))
            //    return new PlayerTypeConverter();
            return TypeDescriptor.GetConverter(type);
        }

        public static object ConvertFromWithServiceContext(this TypeConverter converter, IServiceProvider serviceProvider, object value)
        {
            return converter.ConvertFrom(new ServiceProviderTypeDescriptor(serviceProvider), Thread.CurrentThread.CurrentCulture, value);
        }
    }
}
