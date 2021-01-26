using System;
using System.Collections.Generic;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents a registered command.
    /// </summary>
    public interface ICommandRegistration
    {
        /// <value>
        /// The owner component of the command.
        /// </value>
        IOpenModComponent Component { get; }

        /// <value>
        /// The unique ID of the command.
        /// </value>
        string Id { get; }

        /// <value>
        ///     <para>The primary name of the command, which will be used to execute it.</para>
        ///     <para>The primary name overrides any <see cref="Aliases">aliases</see> of other commands by default.</para>
        /// </value>
        /// <example>
        ///     If the name is "heal", the command will be usually be called using "/heal" (or just "heal" in console)
        /// </example>
        string Name { get; }

        /// <value>
        /// The aliases of the command, which are often shorter versions of the primary name.
        /// </value>
        /// <example>
        /// If the aliases are "h" and "he", the command can be executed using "/h" or "/he".
        /// </example>
        IReadOnlyCollection<string>? Aliases { get; }

        /// <summary>
        /// The permission registrations for this command.
        /// </summary>
        IReadOnlyCollection<IPermissionRegistration>? PermissionRegistrations { get; }

        /// <value>
        /// The description of the command. Can be null or empty.
        /// </value>
        string? Description { get; }

        /// <value>
        ///     The command syntax will be shown to the actor when the command was not used correctly. Can be null or empty.
        ///     <para>An output for the above example could be "/heal [player] &lt;amount&gt;".</para>
        ///     <para>The syntax should not contain Child Command usage.</para>
        /// </value>
        /// <remarks>
        ///     [...] means optional argument and &lt;...&gt; means required argument, so in this case "player" is an optional
        ///     argument while "amount" is a required one.
        /// </remarks>
        /// <example>
        ///     <c>"[player] &lt;amount&gt;"</c>
        /// </example>
        ///
        string? Syntax { get; }

        /// <value>
        /// The priority for this command. Used in case of conflicting commands for determining which command to execute.
        /// The command with higher priority will be preferred.
        /// </value>
        Priority Priority { get; }

        /// <summary>
        /// The ID of the parent command. Can be null if this command does not have a parent command.
        /// </summary>
        string? ParentId { get; }

        /// <value>
        /// <b>True</b> if the command is enabled; otherwise, <b>false</b>.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Checks if the given actor can use this command.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        bool SupportsActor(ICommandActor actor);

        /// <summary>
        /// Instantiates a new command instance for execution.
        /// </summary>
        /// <param name="serviceProvider">The service provider of the command scope.</param>
        /// <return>The instantiated command.</return>
        ICommand Instantiate(IServiceProvider serviceProvider);
    }
}