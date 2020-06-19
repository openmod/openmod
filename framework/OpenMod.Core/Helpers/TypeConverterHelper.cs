using System;
using System.ComponentModel;
using System.Threading;

namespace OpenMod.Core.Helpers
{
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
