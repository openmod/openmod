using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace UserDatabasePlugin.Database
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [NotNull]
        public string Type { get; set; }

        public virtual List<UserActivity> UserActivities { get; set; }
    }
}