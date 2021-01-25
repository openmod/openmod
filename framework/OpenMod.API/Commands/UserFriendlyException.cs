using System;

namespace OpenMod.API.Commands
{
    /// <summary>
    /// Represents exceptions of which the message should be shown to the command actor.
    /// </summary>
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
 
        }
    }
}