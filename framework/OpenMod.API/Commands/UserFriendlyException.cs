using System;

namespace OpenMod.API.Commands
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
 
        }
    }
}