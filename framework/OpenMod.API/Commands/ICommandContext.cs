using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    public interface ICommandContext
    {
        /// <summary>
        ///     The parent command context for Child Commands.
        ///     <para>
        ///         <b>This property can return null.</b>
        ///     </para>
        /// </summary>
        /// <example>
        ///     If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        ICommandContext ParentContext { get; }

        /// <summary>
        ///     The child command context.
        /// </summary>
        ICommandContext ChildContext { get; }

        /// <summary>
        ///     The root context.
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommandContext RootContext { get; }

        /// <summary>
        ///     <para>The prefix used to call this (sub) command.</para>
        ///     <para>Useful when sending command usage messages.</para>
        ///     <para>
        ///         Child commands include their parents.
        ///     </para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         If the command was executed using "/mycommand", it will be "/", when it was executed using "!mycommand", it
        ///         will be "!".
        ///     </para>
        ///     <para>
        ///         If the command was a ChildrenCommand "sub", "/mycommand sub" will return "/mycommand" as prefix.
        ///     </para>
        /// </example>
        string CommandPrefix { get; }

        /// <summary>
        ///     The alias or name used to execute this (sub) command.
        /// </summary>
        string CommandAlias { get; }

        /// <summary>
        ///     <para>The actor executing command.</para>
        ///     <para>Is guaranteed to be a <see cref="ICommand.SupportsActor">supported actor</see>.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommandActor Actor { get; }

        /// <summary>
        ///     <para>The parameters of the (sub) command.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommandParameters Parameters { get; }

        /// <summary>
        ///     Prints the command usage to the actor.
        /// </summary>
        Task PrintCommandUsageAsync();

        ICommandRegistration CommandRegistration { get; }

        Exception Exception { get; }

        Dictionary<string, object> Data { get; }
    }
}