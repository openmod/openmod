using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace UserDatabasePlugin.Database
{
    public class User
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        [NotNull]
        [StringLength(20)]
        public string Type { get; set; }

        public virtual List<UserActivity> UserActivities { get; set; }
    }
}