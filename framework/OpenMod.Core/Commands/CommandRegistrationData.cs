using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    /// <summary>
    ///    Used for storing and loading commands from and to data store
    /// </summary>
    [Serializable]
    public class CommandRegistrationData
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        
        [CanBeNull]
        public ICollection<string> Aliases { get; set; }
        
        [CanBeNull]
        public string ParentFullName { get; set; }
    }
}