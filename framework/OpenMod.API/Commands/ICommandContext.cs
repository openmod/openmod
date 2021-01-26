using System;
using System.Collections.Generic;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public interface ICommandContext : IAsyncDisposable
    {
        /// <value>
        /// The parent command context.
        /// </value>
        /// <example>
        /// If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        ICommandContext? ParentContext { get; }

        /// <value>
        /// The child command context.
        /// </value>
        ICommandContext? ChildContext { get; }

        /// <value>
        ///     The root context.
        /// </value>
        ICommandContext RootContext { get; }

        /// <value>
        ///     <para>The prefix used to call the command.</para>
        ///     <para>Useful for sending command usage messages.</para>
        ///     <para>
        ///         Child commands will include their parents.
        ///     </para>
        /// </value>
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

        /// <value>
        /// The alias or name used to execute the command.
        /// </value>
        string CommandAlias { get; }

        /// <value>
        /// The actor executing command.
        /// </value>
        ICommandActor Actor { get; }

        /// <value>
        /// The parameters of the command.
        /// </value>
        ICommandParameters Parameters { get; }

        /// <value>
        /// The command registration. Can be null if the command was not found.
        /// </value>
        ICommandRegistration? CommandRegistration { get; }

        /// <value>
        /// The exception thrown by the command, if one was thrown; otherwise, <b>null</b>.
        /// </value>
        Exception? Exception { get; set; }

        /// <value>
        /// Container for arbitrary data for the command context.
        /// </value>
        Dictionary<string, object> Data { get; }

        /// <value>
        /// The service provider for the command context.
        /// </value>
        IServiceProvider ServiceProvider { get; }
    }
}