using System;
using System.ComponentModel;
using System.Threading;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;

namespace OpenMod.Core.Helpers
{
    [OpenModInternal]
    public static class TypeConverterHelper
    {
        public static TypeConverter GetConverter(Type type)
        {
            if (typeof(IUser).IsAssignableFrom(type))
                return new UserTypeConverter();

            return TypeDescriptor.GetConverter(type);
        }

        public static object ConvertFromWithServiceContext(this TypeConverter converter, IServiceProvider serviceProvider, object value)
        {
            return converter.ConvertFrom(new ServiceProviderTypeDescriptor(serviceProvider), Thread.CurrentThread.CurrentCulture, value);
        }
    }
}
