using System;

namespace OpenMod.Core.Commands
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
 
        }
    }
}