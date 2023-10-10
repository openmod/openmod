using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents the parameters of a command.
    /// </summary>
    /// <remarks>
    ///  When a command was entered as "/mycommand test 5 b", this class will handle and represent "test", "5" and "b".
    /// </remarks>
    public interface ICommandParameters : IReadOnlyCollection<string>
    {
        /// <summary>
        ///     Gets the n. command parameter starting from zero.
        ///     <para>Index must be less than <see cref="Length">length</see> and not negative.</para>
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     When <i>index</i> is equal or greater than
        ///     <see cref="Length">length</see> or negative.
        /// </exception>
        /// <param name="index">The zero-based index of the parameter.</param>
        /// <seealso cref="T:System.ArgumentOutOfRangeException" />
        /// <returns>The n. command parameter as string.</returns>
        string this[int index] { get; }

        /// <summary>
        /// Gets the amount of parameters.
        /// </summary>
        /// <example>
        /// If the command was entered as "/mycommand test 5 b", it would return "3".
        /// </example>
        int Length { get; }

        /// <summary>
        ///     <para>Gets the parameter value at the given index. The value will parsed to the given type.</para>
        ///     <para>Types like <i>IPlayer</i>, <i>IOnlinePlayer</i>, etc. are supported.</para>
        /// </summary>
        /// <example>
        ///     Assume the command was entered as "/mycommand test 5 b". <br />
        ///     <c>Get&lt;string&gt;(0)</c> would be equal to "test" (string). <br />
        ///     <c>Get&lt;int&gt;(1)</c> would be equal to 5 (int). <br />
        ///     <c>Get&lt;string&gt;(1)</c> would be equal to "5" (string). <br />
        ///     <c>Get&lt;string&gt;(2)</c> would be equal to "b" (string).
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     when <i>index</i> is equal or greater than
        ///     <see cref="Length">length</see> or negative.
        /// </exception>
        /// <typeparam name="T">The type to parse the parameter as.</typeparam>
        /// <param name="index">The zero-based parameter index.</param>
        /// <returns>The parsed parameter value.</returns>
        Task<T> GetAsync<T>(int index);

        /// <summary>
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </exception>
        /// <param name="index">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </param>
        /// <param name="type">The type to parse the parameter as.</param>
        /// <returns>
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </returns>
        Task<object> GetAsync(int index, Type type);

        /// <summary>
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </summary>
        /// <typeparam name="T">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </typeparam>
        /// <param name="index">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </param>
        /// <param name="defaultValue">The default return value.</param>
        /// <returns>
        ///     the parsed parameter value if the given index was valid and the parameter could be parsed to the given type;
        ///     otherwise <i>defaultValue</i>.
        /// </returns>
        Task<T?> GetAsync<T>(int index, T? defaultValue);

        /// <summary>
        ///     <inheritdoc cref="GetAsync{T}(int, T)" />
        /// </summary>
        /// <param name="index">
        ///     <inheritdoc cref="GetAsync{T}(int, T)" />
        /// </param>
        /// <param name="type">The type to parse the parameters as.</param>
        /// <param name="defaultValue">The default return value.</param>
        /// <returns>
        ///     <inheritdoc cref="GetAsync{T}(int, T)" />
        /// </returns>
        Task<object?> GetAsync(int index, Type type, object? defaultValue);

        /// <summary>
        ///     Tries to get and parse a parameter. See <see cref="GetAsync{T}(int)" />.
        /// </summary>
        /// <typeparam name="T">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </typeparam>
        /// <param name="index">
        ///     <inheritdoc cref="GetAsync{T}(int)" />
        /// </param>
        /// <param name="value">The parsed parameter value.</param>
        /// <returns>
        ///     <b>true</b> if the given index was valid and the parameter could be parsed to the given type; otherwise
        ///     <b>false</b>.
        /// </returns>
        bool TryGet<T>(int index, out T? value);

        /// <summary>
        ///     Returns the joined arguments starting from the given position.
        /// </summary>
        /// <example>
        ///     If the command was entered as "/mycommand dosomething a bla c" it would return "a bla c" if startPosition was 1.
        /// </example>
        /// <param name="startPosition">The zero based position to start from.</param>
        /// <exception cref="IndexOutOfRangeException">If startPosition is greater than or equal <see cref="Length">length</see>.</exception>
        /// <returns>he joined arguments starting from the given position</returns>
        string GetArgumentLine(int startPosition);

        /// <summary>
        ///     Returns the joined arguments starting at the given position.
        /// </summary>
        /// <example>
        ///     If the command was entered as "/mycommand dosomething a bla c" it would return "a bla" if startPosition was 1 and
        ///     endPosition was 2.
        /// </example>
        /// <param name="startPosition">The zero based position to start from.</param>
        /// <param name="endPosition">The end position.</param>
        /// <exception cref="IndexOutOfRangeException">
        ///     If startPosition or endPosition is greater than or equal
        ///     <see cref="Length">length</see>.
        /// </exception>
        /// <exception cref="ArgumentException">If startPosition is greater than or equal endPosition.</exception>
        /// <exception cref="IndexOutOfRangeException">If startPosition or endPosition is greather than <see cref="Length"/>.</exception>
        /// <returns>he joined arguments starting from the given position</returns>
        string GetArgumentLine(int startPosition, int endPosition);

        /// <summary>
        ///     <inheritdoc cref="TryGet{T}" />
        /// </summary>
        /// <param name="index">
        ///     <inheritdoc cref="TryGet{T}" />
        /// </param>
        /// <param name="type">The type to parse the parameters as.</param>
        /// <param name="value">The parsed parameter value.</param>
        /// <returns>
        ///     <inheritdoc cref="TryGet{T}" />
        /// </returns>
        bool TryGet(int index, Type type, out object? value);

        /// <summary>
        ///     Gets the parameters as string array.
        /// </summary>
        /// <returns>the parameters as string array.</returns>
        string[] ToArray();

        /// <summary>
        ///     Gets the parameters as string list.
        /// </summary>
        /// <returns>the parameters as string list.</returns>
        List<string> ToList();
    }
}