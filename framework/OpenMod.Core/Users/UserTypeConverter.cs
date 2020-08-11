using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Users
{
    /// <summary>
    /// Converts <i>actorType:actorNameOrId</i> (for example <i>player:Foo Bar</i>)
    /// to <see cref="IUser"/> and vice versa.
    /// The <see cref="UserTypeConverter.DefaultActorType"/> will be used if none is specified.
    /// </summary>
    public sealed class UserTypeConverter : TypeConverter
    {
        public string DefaultActorType;
        public char Separator;

        public UserTypeConverter(string defaultActorType = KnownActorTypes.Player, char separator = ':')
        {
            DefaultActorType = defaultActorType;
            Separator = separator;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string arg))
                return base.ConvertFrom(context, culture, value);

            IUserManager userManager = context.GetRequiredService<IUserManager>();

            string userActorType;
            string userNameOrId;

            if (arg.Contains(Separator))
            {
                string[] args = arg.Split(Separator);
                userActorType = args[0];
                userNameOrId = string.Join(Separator.ToString(), args.Skip(1).ToArray());
            }
            else
            {
                userActorType = DefaultActorType;
                userNameOrId = arg;
            }

            return AsyncHelper.RunSync(async ()
                => await userManager.FindUserAsync(userActorType, userNameOrId, UserSearchMode.FindByNameOrId));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IUser user)
                return user.Type + Separator + user.Id;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
