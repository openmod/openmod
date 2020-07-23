using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    public class CommandParameters : ICommandParameters, IEnumerable<string>
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IOpenModStringLocalizer m_OpenModStringLocalizer;

        public CommandParameters(CommandContext commandContext, ICollection<string> args)
        {
            m_OpenModStringLocalizer = commandContext.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>();
            m_ServiceProvider = commandContext.ServiceProvider;
            RawParameters = args;
        }

        /// <summary>
        ///     The internal stored raw parameter list
        /// </summary>
        protected ICollection<string> RawParameters { get; }

        /// <inheritdoc />
        public string this[int index] => ToArray()[index];

        /// <inheritdoc />
        public int Length => ToArray().Length;

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(int index) => (T)await GetAsync(index, typeof(T));

        /// <inheritdoc />
        public async Task<object> GetAsync(int index, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (Length <= index)
            {
                throw new CommandIndexOutOfRangeException(m_OpenModStringLocalizer["commands:errors:out_of_range_error", new { Index = index, Type = type, Length }], index, Length);
            }

            if (index < 0)
                throw new IndexOutOfRangeException();

            string arg = ToArray()[index];



            //todo make type converters
            //if (typeof(IUser).IsAssignableFrom(type))
            //{
            //    /*
            //     * The logic for getting IUser and IUserInfo is as follows:
            //     * If the name is supplied as usermanager:username, e.g. discord:Trojaner, it will search for valid "discord" user manager and return the "Trojaner" named user of it.
            //     * Otherwise (if the user manager does not exist or the format was not supplied), it will use the player manager and return the user for the given input.
            //     */
            //    IUserManager targetUserManager = container.Resolve<IPlayerManager>();

            //    string userName = arg;
            //    if (arg.Contains(":"))
            //    {
            //        var args = arg.Split(':');
            //        string userManagerMapping = args.First();

            //        foreach (var userMgr in container.ResolveAll<IUserManager>())
            //        {
            //            if (userMgr.ServiceName.Equals(userManagerMapping, StringComparison.OrdinalIgnoreCase))
            //            {
            //                userName = string.Join(":", args.Skip(1).ToArray());
            //                targetUserManager = userMgr;
            //                break;
            //            }
            //        }
            //    }

            //    return await targetUserManager.GetUserAsync(userName);
            //}

            TypeConverter converter = TypeConverterHelper.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return converter.ConvertFromWithServiceContext(m_ServiceProvider, arg);
                }
                catch (Exception ex)
                {
                    if (!(ex.InnerException is FormatException))
                    {
                        throw;
                    }
                }
            }

            throw new CommandParameterParseException(m_OpenModStringLocalizer["commands:errors:parse_error", new { Value = arg , Type = type}], arg, type);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(int index, T defaultValue) => (T)await GetAsync(index, typeof(T), defaultValue);

        /// <inheritdoc />
        public async Task<object> GetAsync(int index, Type type, object defaultValue)
        {
            if (TryGet(index, type, out object val))
                return val;
            return defaultValue;
        }

        /// <inheritdoc />
        public bool TryGet<T>(int index, out T value)
        {
            bool result = TryGet(index, typeof(T), out object tmp);
            if (result)
            {
                value = (T) tmp;
            }
            else
            {
                value = default;
            }

            return result;
        }

        /// <inheritdoc />
        public string GetArgumentLine(int startPosition)
        {
            if (startPosition > Length)
                throw new IndexOutOfRangeException();

            return string.Join(" ", ToArray().Skip(startPosition).ToArray());
        }

        /// <inheritdoc />
        public string GetArgumentLine(int startPosition, int endPosition)
        {
            if (startPosition > Length)
                throw new IndexOutOfRangeException();

            if (endPosition > Length)
                throw new IndexOutOfRangeException();

            if (endPosition - startPosition < 1)
                throw new ArgumentException();

            return string.Join(" ", ToArray().Skip(startPosition).Take(endPosition - startPosition).ToArray());
        }

        /// <inheritdoc />
        public bool TryGet(int index, Type type, out object value)
        {
            value = null;
            try
            {
                value = GetAsync(index, type).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public string[] ToArray() => RawParameters.ToArray();

        /// <inheritdoc />
        public List<string> ToList() => ToArray().ToList();

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => ToList().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public int Count
        {
            get { return RawParameters.Count; }
        }
    }
}