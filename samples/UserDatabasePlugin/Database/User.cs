using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserDatabasePlugin.Database
{
    public class User
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = null!;

        public virtual List<UserActivity> UserActivities { get; set; } = null!;
    }
}