using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public interface ICommandContext : IAsyncDisposable
    {
        /// <value>
        /// The parent command context. Can be null.
        /// </value>
        /// <example>
        /// If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        [CanBeNull]
        ICommandContext ParentContext { get; }

        /// <value>
        /// The child command context. Can be null.
        /// </value>
        [CanBeNull]
        ICommandContext ChildContext { get; }

        /// <value>
        ///     The root context. Cannot be null.
        /// </value>
        [NotNull]
        ICommandContext RootContext { get; }

        /// <value>
        ///     <para>The prefix used to call the command. Cannot be null.</para>
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
        [NotNull]
        string CommandPrefix { get; }

        /// <value>
        /// The alias or name used to execute the command. Cannot be null.
        /// </value>
        [NotNull]
        string CommandAlias { get; }

        /// <value>
        /// The actor executing command. Cannot be null.
        /// </value>
        [NotNull]
        ICommandActor Actor { get; }

        /// <value>
        /// The parameters of the command. Cannot be null.
        /// </value>
        [NotNull]
        ICommandParameters Parameters { get; }

        /// <value>
        /// The command registration. Can be null if the command was not found.
        /// </value>
        [CanBeNull]
        ICommandRegistration CommandRegistration { get; }

        /// <value>
        /// The exception thrown by the command, if one was thrown; otherwise, <b>null</b>.
        /// </value>
        [CanBeNull]
        Exception Exception { get; set; }

        /// <value>
        /// Container for arbitrary data for the command context. Cannot be null.
        /// </value>
        [NotNull]
        Dictionary<string, object> Data { get; }

        /// <value>
        /// The service provider for the command context. Cannot be null.
        /// </value>
        [NotNull]
        IServiceProvider ServiceProvider { get; }
    }
}