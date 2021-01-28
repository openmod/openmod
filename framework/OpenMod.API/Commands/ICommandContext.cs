using System;
using System.Collections.Generic;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public interface ICommandContext : IAsyncDisposable
    {
        /// <summary>
        /// Gets the parent command context.
        /// </summary>
        /// <example>
        /// If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        ICommandContext? ParentContext { get; }

        /// <summary>
        /// Gets the child command context.
        /// </summary>
        ICommandContext? ChildContext { get; }

        /// <summary>
        /// Gets the root context.
        /// </summary>
        ICommandContext RootContext { get; }

        /// <summary>
        ///     <para>Gets the prefix used to call the command.</para>
        ///     <para>Useful for sending command usage messages.</para>
        ///     <para>
        ///         Child commands will include their parents.
        ///     </para>
        /// </summary>
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
        /// Gets the alias or name used to execute the command.
        /// </summary>
        string CommandAlias { get; }

        /// <summary>
        /// Gets the actor executing command.
        /// </summary>
        ICommandActor Actor { get; }

        /// <summary>
        /// Gets the parameters of the command.
        /// </summary>
        ICommandParameters Parameters { get; }

        /// <summary>
        /// Gets the command registration. Returns null if the command was not found.
        /// </summary>
        ICommandRegistration? CommandRegistration { get; }

        /// <summary>
        /// Gets the exception thrown by the command if one was thrown; otherwise, <b>null</b>.
        /// </summary>
        Exception? Exception { get; set; }

        /// <summary>
        /// Gets the data for the context. Can be used by plugins for passing arbritrary data to the command context.
        /// </summary>
        Dictionary<string, object> Data { get; }

        /// <summary>
        /// Gets the service provider for the command context.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}