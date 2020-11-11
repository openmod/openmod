using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    public class CommandParameters : ICommandParameters, IEnumerable<string>
    {
        private readonly IOpenModStringLocalizer m_OpenModStringLocalizer;
        private readonly ICommandParameterResolver m_CommandParameterResolver;

        public CommandParameters(CommandContext commandContext, ICollection<string> args)
        {
            m_OpenModStringLocalizer = commandContext.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>();
            RawParameters = args;
            m_CommandParameterResolver = commandContext.ServiceProvider.GetRequiredService<ICommandParameterResolver>();
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
            object value = await m_CommandParameterResolver.ResolveAsync(type, arg);

            if (value != null)
            {
                return value;
            }

            throw new CommandParameterParseException(m_OpenModStringLocalizer["commands:errors:parse_error", new { Value = arg, Type = type }], arg, type);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(int index, T defaultValue) => (T)await GetAsync(index, typeof(T), defaultValue);

        /// <inheritdoc />
        public Task<object> GetAsync(int index, Type type, object defaultValue)
        {
            if (TryGet(index, type, out object val))
                return Task.FromResult(val);

            return Task.FromResult(defaultValue);
        }

        /// <inheritdoc />
        public bool TryGet<T>(int index, out T value)
        {
            bool result = TryGet(index, typeof(T), out object tmp);
            if (result)
            {
                value = (T)tmp;
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